using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using LettuceIo.Dotnet.Base.Extensions;
using LettuceIo.Dotnet.Core.Enums;
using LettuceIo.Dotnet.Core.Interfaces;
using LettuceIo.Dotnet.Core.Structs;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace LettuceIo.Dotnet.Base.Actions
{
    public class Publish : IAction
    {
        public Status Status { get; private set; } = Status.Pending;
        public IObservable<Metrics> Metrics => _statsSubject;

        #region fields

        private readonly IConnectionFactory _connectionFactory;
        private readonly Limits _limits;
        private readonly string _exchange;
        private readonly string _folderPath;
        private readonly PublishOptions _options;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly TimeSpan _updateInterval;
        private Metrics _currentMetrics = new Metrics {Count = 0, Duration = TimeSpan.Zero, SizeKB = 0d};
        private readonly ISubject<Metrics> _statsSubject = new Subject<Metrics>();
        private readonly Random _random = new Random();
        private readonly Stopwatch _durationStopWatch = new Stopwatch();
        private IDisposable? _updateTick;
        private readonly List<Task> _publishTasks = new List<Task>();
        private IConnection? _connection;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _disposed;

        #endregion

        public Publish(IConnectionFactory connectionFactory, Limits limits, string exchange,
            string folderPath, PublishOptions options, JsonSerializerSettings serializerSettings,
            TimeSpan updateInterval)
        {
            _connectionFactory = connectionFactory;
            _limits = limits;
            _exchange = exchange;
            _folderPath = folderPath;
            _options = options;
            _serializerSettings = serializerSettings;
            _updateInterval = updateInterval;
        }

        public void Start()
        {
            if (Status != Status.Pending) throw new InvalidOperationException("The action is not pending activation.");
            Status = Status.Running;

            IEnumerable<Message> messages = LoadMessages();
            if (_options.Shuffle) messages = messages.Shuffle(_random);
            _connection = _connectionFactory.CreateConnection();

            if (_options.Playback)
            {
                if (_options.Loop) messages = messages.Loop();
                messages = messages.Select(RoutingKeyModifier()); //Handle RoutingKeyOptions
                _publishTasks.Add(new Task(() =>
                {
                    var channel = _connection.CreateModel();
                    var timer = new Timer();
                    foreach (var message in messages)
                    {
                        //check limits
                        if (_cts.IsCancellationRequested || LimitsReached()) break;

                        //sleep before publish for current message timeDelta
                        timer.Sleep(message.TimeDelta);

                        //publish
                        channel.BasicPublish(_exchange, message);

                        //update metrics
                        UpdateMetrics(message);
                    }

                    channel.Abort();
                }, _cts.Token));
            }
            else
            {
                var intervalMilliSeconds = 1000d / _options.RateDetails.RateHz;
                IEnumerable<IEnumerable<Message>> buckets = messages.Split(_options.RateDetails.Multiplier).ToList();
                if (_options.Loop) buckets = buckets.Select(EnumerableExtensions.Loop);
                buckets = buckets.Select(bucket => bucket.Select(RoutingKeyModifier())); //Handle RoutingKeyOptions
                _publishTasks.AddRange(buckets.Select(bucket => new Task(() =>
                {
                    var channel = _connection.CreateModel();
                    var timer = new Timer();
                    foreach (var message in bucket)
                    {
                        //check limits
                        if (_cts.IsCancellationRequested || LimitsReached()) break;

                        //publish
                        channel.BasicPublish(_exchange, message);
                        // channel.WaitForConfirms();

                        //update metrics
                        UpdateMetrics(message);

                        //sleep
                        timer.Sleep(
                            intervalMilliSeconds); //TODO: maybe remove sw handling so it is more precise here...
                    }

                    channel.Abort();
                }, _cts.Token)).ToArray());
            }

            //Notify if error and stop when any of the tasks finish
            Task.WhenAny(_publishTasks).ContinueWith(task =>
            {
                if (task.Result.IsFaulted) OnError(task.Result.Exception!);
                else if (task.Result.IsCompleted) Stop();
            }, _cts.Token);

            //Start all
            _durationStopWatch.Start();
            _updateTick = Observable.Interval(_updateInterval).Subscribe(_ =>
            {
                _currentMetrics.Duration = _durationStopWatch.Elapsed;
                _statsSubject.OnNext(_currentMetrics);
            });
            _publishTasks.ForEach(task => task.Start());
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

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _cts.Cancel();
            _updateTick?.Dispose();
            _connection?.Close();
        }

        private bool LimitsReached() => _limits.Amount <= _currentMetrics.Count ||
                                        _limits.Duration <= _currentMetrics.Duration ||
                                        _limits.SizeKB <= _currentMetrics.SizeKB;

        private void UpdateMetrics(Message message)
        {
            _currentMetrics.Count++;
            _currentMetrics.Duration = _durationStopWatch.Elapsed;
            _currentMetrics.SizeKB += message.SizeKB();
        }

        private IEnumerable<Message> LoadMessages()
        {
            var files = Directory.EnumerateFiles(_folderPath, "*.json").ToArray();
            if (files.Length <= 0) throw new Exception("No files in folder");
            return files.Select(file =>
            {
                var text = File.ReadAllText(file);
                var message = JsonConvert.DeserializeObject<Message?>(text, _serializerSettings);
                return message ?? throw new Exception($"Invalid file in folder (Filename: \"{file}\")");
            });
        }

        private Func<Message, Message> RoutingKeyModifier() => _options.RoutingKeyDetails.RoutingKeyType switch
        {
            PublishRoutingKeyType.Random => message =>
            {
                message.RoutingKey =
                    _random.Next().ToString(); /*TODO: check if is random with full string space for C.H exchange*/
                return message;
            },
            PublishRoutingKeyType.Custom => message =>
            {
                message.RoutingKey = _options.RoutingKeyDetails.CustomValue;
                return message;
            },
            _ => message => message
        };
    }
}