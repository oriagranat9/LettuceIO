using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using LettuceIo.Dotnet.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LettuceIo.Dotnet.Base.Actions
{
    public class Record : IAction
    {
        public IObservable<ActionMetrics> Stats => _statsSubject;

        #region fields

        private readonly IModel _channel;
        private readonly ISubject<ActionMetrics> _statsSubject = new Subject<ActionMetrics>();
        private string? _consumerTag;
        private IDisposable? _subscription;
        private readonly JToken _limit = new JObject(); //TODO
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private string queueName = ""; //TODO
        private ActionMetrics _currentMetrics;
        private readonly TimeSpan _updateEvery = TimeSpan.FromSeconds(1); //TODO
        private bool _started;
        private JsonSerializerSettings _jsonSerializerSettings; //TODO
        private Encoding _encoding = Encoding.UTF8; //TODO

        #endregion

        public Record(JToken settings)
        {
            _channel = settings.Value<ConnectionFactory>().CreateConnection().CreateModel();
        }

        public void Start()
        {
            if (_started) throw new InvalidOperationException("The action was started already");
            _started = true;

            var consumer = new EventingBasicConsumer(_channel);
            _subscription = Observable.FromEventPattern<BasicDeliverEventArgs>(handler => consumer.Received += handler,
                    handler => consumer.Received -= handler)
                .TimeInterval()
                .Limit(_limit)
                .Subscribe(OnMessage, Stop);
            _stopwatch.Restart();
            Observable.Interval(_updateEvery).Subscribe(_ =>
            {
                _currentMetrics.Duration = _stopwatch.Elapsed;
                _statsSubject.OnNext(_currentMetrics);
            });
            _currentMetrics = new ActionMetrics {Count = 0, Duration = TimeSpan.Zero, SizeKB = 0d};
            _consumerTag = _channel.BasicConsume(consumer, queueName, true, exclusive: true);
        }

        public void Stop()
        {
            _subscription?.Dispose();
            _statsSubject.OnCompleted();
            _channel.BasicCancel(_consumerTag);
        }

        private void OnMessage(TimeInterval<EventPattern<BasicDeliverEventArgs>> obj)
        {
            //serialize message
            var message = new Message
            {
                Body = obj.Value.EventArgs.Body.ToArray(),
                RoutingKey = obj.Value.EventArgs.RoutingKey,
                TimeDelta = obj.Interval
            };
            var json = JsonConvert.SerializeObject(message, _jsonSerializerSettings);
            var sizeKB = _encoding.GetByteCount(json) / 1024d;

            //update stats
            _currentMetrics.Duration = _stopwatch.Elapsed;
            _currentMetrics.Count++;
            _currentMetrics.SizeKB += sizeKB;
            _statsSubject.OnNext(_currentMetrics);

            //write message to file
            throw new NotImplementedException();
        }
    }
}