using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using LettuceIo.Dotnet.Core;
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
        public IObservable<ActionMetrics> Stats => _statsSubject;
        private readonly ISubject<ActionMetrics> _statsSubject = new Subject<ActionMetrics>();
        private bool _started;
        private IModel? _channel;
        private Random _random = new Random();
        private bool _stop;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public Publish(IConnectionFactory connectionFactory, Limits limits, string exchange,
            String folderPath, PublishOptions options, JsonSerializerSettings serializerSettings)
        {
            _connectionFactory = connectionFactory;
            _limits = limits;
            _exchange = exchange;
            _options = options;
            var collection = new Collection<Message>();
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.json"))
            {
                collection.Add(JsonConvert.DeserializeObject<Message>(file, serializerSettings));
            }

            _messages = collection;
        }

        public void Start()
        {
            if (_started) throw new InvalidOperationException("The action was started already");
            _started = true;
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
            _stop = true;
            _cts.Cancel();
        }

        private void BasicPublish(Message message) =>
            _channel.BasicPublish(_exchange, message.RoutingKey, body: message.Body);
    }
}