using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IConnectionFactory _connectionFactory;
        private readonly Limits _limits;
        private readonly string _exchange;
        private readonly IReadOnlyCollection<Message> _messages;
        private readonly PublishOptions _options;
        public Status Status { get; private set; } = Status.Pending;
        public IObservable<ActionMetrics> Stats => _statsSubject;
        private readonly ISubject<ActionMetrics> _statsSubject = new Subject<ActionMetrics>();
        private IModel? _channel;
        private Random _random = new Random();
        private bool _stop;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public Publish(IConnectionFactory connectionFactory, Limits limits, string exchange,
            string folderPath, PublishOptions options, JsonSerializerSettings serializerSettings)
        {
            _connectionFactory = connectionFactory;
            _limits = limits;
            _exchange = exchange;
            _options = options;
            _messages = Directory.EnumerateFiles(folderPath, "*.json")
                .Select(File.ReadAllText)
                .Select(text => JsonConvert.DeserializeObject<Message>(text, serializerSettings))
                .ToArray() as Message[] ?? throw new ArgumentException("No messages in directory");
        }

        public void Start()
        {
            if (Status != Status.Pending) throw new InvalidOperationException("The action was started already");
            Status = Status.Running;
            _channel = _connectionFactory.CreateConnection().CreateModel();

            IEnumerable<Message> messages = _messages;
            if (_options.Shuffle) messages = messages.Shuffle();
            if (_options.Loop) messages = messages.Loop();

            if (_options.Playback)
                Task.Run(() =>
                {
                    foreach (var message in messages)
                    {
                        if (_stop) return;
                        Timer.Sleep(message.TimeDelta);
                        BasicPublish(message);
                    }
                }, _cts.Token);
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Stop()
        {
            if (Status == Status.Stopped) return;
            if (Status != Status.Running) throw new InvalidOperationException("The action is not running");
            Status = Status.Stopped;
            _cts.Cancel();
        }

        private void BasicPublish(Message message) =>
            _channel.BasicPublish(_exchange, message.RoutingKey, body: message.Body);
    }
}