using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
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
        public IObservable<Metrics> Metrics => _statsSubject;

        #region fields

        private readonly IConnectionFactory _connectionFactory;
        private readonly Limits _limits;
        private readonly string? _exchange;
        private readonly string _queue;
        private readonly RecordOptions _options;
        private readonly string _folderPath;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly TimeSpan _updateInterval;
        private Metrics _currentMetrics = new Metrics {Count = 0, Duration = TimeSpan.Zero, SizeKB = 0d};
        private readonly ISubject<Metrics> _statsSubject = new Subject<Metrics>();
        private readonly Stopwatch _durationStopWatch = new Stopwatch();
        private IDisposable? _consumption;
        private IDisposable? _consumerSubscription;
        private IDisposable? _updateTick;
        private IConnection? _connection;
        private bool _disposed;

        #endregion

        public Record(IConnectionFactory connectionFactory, Limits limits, string? exchange, string queue,
            string folderPath, RecordOptions options, JsonSerializerSettings serializerSettings,
            TimeSpan updateInterval)
        {
            _connectionFactory = connectionFactory;
            _limits = limits;
            _exchange = exchange;
            _queue = queue;
            _options = options;
            _folderPath = folderPath;
            _serializerSettings = serializerSettings;
            _updateInterval = updateInterval;
        }

        public void Start()
        {
            if (Status != Status.Pending) throw new InvalidOperationException("The action is not pending activation.");
            Status = Status.Running;
            _connection = _connectionFactory.CreateConnection();
            var channel = _connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);
            _consumerSubscription = Observable.FromEventPattern<BasicDeliverEventArgs>(
                    handler => consumer.Received += handler,
                    handler => consumer.Received -= handler)
                .TimeInterval()
                .Select(ToMessage)
                .Limit(_limits)
                .Subscribe(OnMessage, Stop);
            _durationStopWatch.Restart();
            _updateTick = Observable.Interval(_updateInterval).Subscribe(_ =>
            {
                _currentMetrics.Duration = _durationStopWatch.Elapsed;
                _statsSubject.OnNext(_currentMetrics);
            });

            if (_exchange != null)
            {
                channel.QueueDeclare(_queue);
                channel.QueueBind(_queue, _exchange, _options.BindingRoutingKey);
            }

            var tag = channel.BasicConsume(consumer, _queue, true);
            _consumption = Disposable.Create(() =>
            {
                channel.BasicCancel(tag);
                channel.Close();
            });
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _consumption?.Dispose();
            _consumerSubscription?.Dispose();
            _connection?.Close();
            _updateTick?.Dispose();
        }

        public void Stop()
        {
            if (Status == Status.Stopped) return;
            Status = Status.Stopped;
            _statsSubject.OnCompleted();
            Dispose();
        }


        private void OnError(Exception exception)
        {
            if (Status == Status.Stopped) return;
            Status = Status.Stopped;
            _statsSubject.OnError(exception);
            Dispose();
        }

        private void OnMessage(Message message)
        {
            var name = $"{_queue}_{DateTime.Now:dd-MM-yyy HH-mm-ss-fff}.json";
            var path = Path.Combine(_folderPath, name);
            //TODO message save as base 64 string ?
            var task = File.WriteAllTextAsync(path, JsonConvert.SerializeObject(message, _serializerSettings));
            task.ContinueWith(t => OnError(t.Exception!), TaskContinuationOptions.OnlyOnFaulted);
            _currentMetrics.Duration = _durationStopWatch.Elapsed;
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