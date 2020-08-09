using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LettuceIo.Dotnet.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LettuceIo.Dotnet.Base.Actions
{
    public class Record : IAction
    {
        public IObservable<ActionMetrics> Stats => _statsSubject;

        public IObservable<Message> Messages => _messageSubject;

        public TimeSpan UpdateEvery = TimeSpan.FromSeconds(1);


        #region fields

        private readonly IConnectionFactory _connectionFactory;
        private readonly Limits _limits;
        private readonly string _queue;
        private IModel? _channel;
        private ActionMetrics _currentMetrics;
        private bool _started;
        private readonly ISubject<ActionMetrics> _statsSubject = new Subject<ActionMetrics>();
        private readonly ISubject<Message> _messageSubject = new Subject<Message>();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private string? _consumerTag;
        private IDisposable? _subscription;
        private IDisposable? _timerSubscription;

        #endregion

        public Record(IConnectionFactory connectionFactory, Limits limits, string queue)
        {
            _connectionFactory = connectionFactory;
            _limits = limits;
            _queue = queue;
        }

        public void Start()
        {
            if (_started) throw new InvalidOperationException("The action was started already");
            _started = true;
            _channel = _connectionFactory.CreateConnection().CreateModel();
            var consumer = new EventingBasicConsumer(_channel);
            _subscription = Observable.FromEventPattern<BasicDeliverEventArgs>(handler => consumer.Received += handler,
                    handler => consumer.Received -= handler)
                .TimeInterval()
                .Select(ToMessage)
                .Limit(_limits)
                .Subscribe(OnMessage, Stop);
            _stopwatch.Restart();
            _timerSubscription = Observable.Interval(UpdateEvery).Subscribe(_ =>
            {
                _currentMetrics.Duration = _stopwatch.Elapsed;
                _statsSubject.OnNext(_currentMetrics);
            });
            _currentMetrics = new ActionMetrics {Count = 0, Duration = TimeSpan.Zero, SizeKB = 0d};
            _consumerTag = _channel.BasicConsume(consumer, _queue, true, exclusive: true);
        }

        public void Stop()
        {
            _subscription?.Dispose();
            _timerSubscription?.Dispose();
            _statsSubject.OnCompleted();
            _messageSubject.OnCompleted();
            _channel?.BasicCancel(_consumerTag);
        }

        private void OnMessage(Message message)
        {
            _messageSubject.OnNext(message);
            _currentMetrics.Duration = _stopwatch.Elapsed;
            _currentMetrics.Count++;
            _currentMetrics.SizeKB += message.SizeKB();
            _statsSubject.OnNext(_currentMetrics);
        }

        private static Message ToMessage(TimeInterval<EventPattern<BasicDeliverEventArgs>> obj) => new Message
        {
            Body = obj.Value.EventArgs.Body.ToArray(),
            RoutingKey = obj.Value.EventArgs.RoutingKey,
            TimeDelta = obj.Interval
        };
    }
}