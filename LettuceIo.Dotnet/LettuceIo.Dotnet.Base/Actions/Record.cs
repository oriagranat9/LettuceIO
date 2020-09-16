using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LettuceIo.Dotnet.Base.Extensions;
using LettuceIo.Dotnet.Core.Enums;
using LettuceIo.Dotnet.Core.Interfaces;
using LettuceIo.Dotnet.Core.Structs;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LettuceIo.Dotnet.Base.Actions
{
    public class Record : IAction
    {
        public Status Status { get; private set; } = Status.Pending;
        public IObservable<ActionMetrics> Stats => _statsSubject;

        #region fields

        private readonly IConnectionFactory _connectionFactory;
        private readonly Limits _limits;
        private readonly string _queue;
        private readonly string _folderPath;
        private readonly JsonSerializerSettings _serializerSettings;
        private IModel? _channel;
        private ActionMetrics _currentMetrics;
        private readonly ISubject<ActionMetrics> _statsSubject = new Subject<ActionMetrics>();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private string? _consumerTag;
        private IDisposable? _subscription;
        private IDisposable? _timerSubscription;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

        #endregion

        public Record(IConnectionFactory connectionFactory, Limits limits, string queue, string folderPath,
            JsonSerializerSettings serializerSettings)
        {
            _connectionFactory = connectionFactory;
            _limits = limits;
            _queue = queue ?? throw new ArgumentException(nameof(queue));
            _folderPath = folderPath ?? throw new ArgumentException(nameof(folderPath));
            _serializerSettings = serializerSettings;
        }

        public void Start()
        {
            if (Status != Status.Pending) throw new InvalidOperationException("The action was started already");
            Status = Status.Running;
            _channel = _connectionFactory.CreateConnection().CreateModel();
            var consumer = new EventingBasicConsumer(_channel);
            _subscription = Observable.FromEventPattern<BasicDeliverEventArgs>(handler => consumer.Received += handler,
                    handler => consumer.Received -= handler)
                .TimeInterval()
                .Select(ToMessage)
                .Limit(_limits)
                .Subscribe(OnMessage, Stop);
            _stopwatch.Restart();
            _timerSubscription = Observable.Interval(_updateInterval).Subscribe(_ =>
            {
                _currentMetrics.Duration = _stopwatch.Elapsed;
                _statsSubject.OnNext(_currentMetrics);
            });
            _currentMetrics = new ActionMetrics {Count = 0, Duration = TimeSpan.Zero, SizeKB = 0d};
            _consumerTag = _channel.BasicConsume(consumer, _queue, true, exclusive: true);
        }

        public void Stop()
        {
            if (Status != Status.Running) throw new InvalidOperationException("The action is not running");
            Status = Status.Stopped;
            _subscription?.Dispose();
            _timerSubscription?.Dispose();
            _statsSubject.OnCompleted();
            _channel?.BasicCancel(_consumerTag);
        }

        private void OnMessage(Message message)
        {
            var name = $"{_queue}_{DateTime.Now:dd-MM-yyy HH-mm-ss-fff}.json";
            var path = Path.Combine(_folderPath, name);
            //TODO message save as base 64 string ?
            var task = File.WriteAllTextAsync(path, JsonConvert.SerializeObject(message, _serializerSettings));
            _currentMetrics.Duration = _stopwatch.Elapsed;
            _currentMetrics.Count++;
            _currentMetrics.SizeKB += message.SizeKB();
            _statsSubject.OnNext(_currentMetrics);
            task.Wait();
        }

        private static Message ToMessage(TimeInterval<EventPattern<BasicDeliverEventArgs>> obj) => new Message
        {
            Body = obj.Value.EventArgs.Body.ToArray(),
            RoutingKey = obj.Value.EventArgs.RoutingKey,
            TimeDelta = obj.Interval
        };
    }
}