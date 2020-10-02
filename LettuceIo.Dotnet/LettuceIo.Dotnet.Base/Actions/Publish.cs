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
        public IObservable<ActionMetrics> Stats => _statsSubject;

        #region fields

        private readonly IConnectionFactory _connectionFactory;
        private readonly Limits _limits;
        private readonly string _exchange;
        private readonly string? _queue;
        private readonly string _folderPath;
        private readonly PublishOptions _options;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ISubject<ActionMetrics> _statsSubject = new Subject<ActionMetrics>();
        private IModel? _channel;
        private readonly Random _random = new Random();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private ActionMetrics _currentMetrics = new ActionMetrics {Count = 0, Duration = TimeSpan.Zero, SizeKB = 0d};
        private readonly Stopwatch _durationStopWatch = new Stopwatch();
        private IDisposable? _timerSubscription;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(100);
        private readonly List<Task> _publishTasks = new List<Task>();

        #endregion

        public Publish(IConnectionFactory connectionFactory, Limits limits, string exchange, string? queue,
            string folderPath, PublishOptions options, JsonSerializerSettings serializerSettings)
        {
            _connectionFactory = connectionFactory;
            _limits = limits;
            _exchange = exchange;
            _queue = queue;
            _folderPath = folderPath;
            _options = options;
            _serializerSettings = serializerSettings;
        }

        public void Start()
        {
            if (Status != Status.Pending) throw new InvalidOperationException("The action was started already");
            Status = Status.Running;

            var arr = Directory.EnumerateFiles(_folderPath, "*.json")
                .Select(File.ReadAllText)
                .Select(text => JsonConvert.DeserializeObject<Message?>(text, _serializerSettings))
                .WhereNotNull()
                .ToArray();
            if (arr.Length == 0) throw new ArgumentException("No messages in directory");

            IEnumerable<Message> messages = arr;
            if (_options.Shuffle) messages = messages.Shuffle(_random);

            //Todo: move to separate method
            //Handle RoutingKeyOptions
            Func<Message, Message> modify = _options.RoutingKeyDetails.RoutingKeyType switch
            {
                PublishRoutingKeyType.Random => message =>
                {
                    message.RoutingKey =
                        _random.Next().ToString(); //TODO: check if is random with full string space for C.H exchange
                    return message;
                },
                PublishRoutingKeyType.Custom => message =>
                {
                    message.RoutingKey = _options.RoutingKeyDetails.CustomValue;
                    return message;
                },
                _ => message => message
            };
            messages = messages.Select(modify);


            var connection = _connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.ModelShutdown += (_, args) => OnError(new Exception(args.ReplyText));

            //Declare extra stuff if needed
            if (_queue != null)
            {
                _channel.ExchangeDeclare(_exchange, "fanout",
                    autoDelete: true); //TODO: make sure exchange is deleted after
                _channel.QueueBind(_queue, _exchange, "");
            }


            if (_options.Playback)
            {
                if (_options.Loop) messages = messages.Loop();
                _publishTasks.Add(new Task(() =>
                {
                    var timer = new Timer();
                    foreach (var message in messages)
                    {
                        //check limits
                        if (_cts.IsCancellationRequested || LimitsReached()) return;

                        //sleep before publish for current message timeDelta
                        timer.Sleep(message.TimeDelta);

                        //publish
                        BasicPublish(_channel, message);

                        //update metrics
                        UpdateMetrics(message);
                    }
                }, _cts.Token));
            }
            else
            {
                var intervalMilliSeconds = 1000d / _options.RateDetails.RateHz;
                IEnumerable<IEnumerable<Message>> buckets = messages.Split(_options.RateDetails.Multiplier).ToList();
                if (_options.Loop) buckets = buckets.Select(EnumerableExtensions.Loop);
                _publishTasks.AddRange(buckets.Select(bucket => new Task(() =>
                {
                    var channel = connection.CreateModel();
                    channel.ModelShutdown += (_, args) => OnError(new Exception(args.ReplyText));
                    var timer = new Timer();
                    foreach (var message in bucket)
                    {
                        //check limits
                        if (_cts.IsCancellationRequested || LimitsReached()) return;

                        //publish
                        BasicPublish(channel, message);

                        //update metrics
                        UpdateMetrics(message);

                        //sleep
                        timer.Sleep(
                            intervalMilliSeconds); //TODO: maybe remove sw handling so it is more precise here...
                    }
                }, _cts.Token)).ToArray());
            }
            // notify if error and stop when any of the tasks finish
            Task.WhenAny(_publishTasks).ContinueWith(task =>
            {
                if (task.IsFaulted) OnError(task.Exception!);
                Stop();
            }, _cts.Token);

            _durationStopWatch.Start();
            _timerSubscription = Observable.Interval(_updateInterval).Subscribe(_ =>
            {
                _currentMetrics.Duration = _durationStopWatch.Elapsed;
                _statsSubject.OnNext(_currentMetrics);
            });
            _publishTasks.ForEach(task => task.Start());
        }

        public void Stop()
        {
            if (Status == Status.Stopped) return;
            if (Status != Status.Running) throw new InvalidOperationException("The action is not running");
            Status = Status.Stopped;
            _cts.Cancel();
            Task.WaitAll(_publishTasks.ToArray());
            _publishTasks.ForEach(task => task.Dispose());
            _statsSubject.OnCompleted();
            _timerSubscription?.Dispose();
            if (_queue != null) _channel.ExchangeDelete(_exchange);
        }

        private void OnError(Exception exception)
        {
            if (Status == Status.Stopped) return;
            _statsSubject.OnError(exception);
            Stop();
        }

        private void BasicPublish(IModel channel, Message message) =>
            channel.BasicPublish(_exchange, message.RoutingKey, body: message.Body);

        private bool LimitsReached() => _limits.Amount <= _currentMetrics.Count ||
                                        _limits.Duration <= _currentMetrics.Duration ||
                                        _limits.SizeKB <= _currentMetrics.SizeKB;

        private void UpdateMetrics(Message message)
        {
            _currentMetrics.Count++;
            _currentMetrics.Duration = _durationStopWatch.Elapsed;
            _currentMetrics.SizeKB += message.SizeKB();
        }
    }
}