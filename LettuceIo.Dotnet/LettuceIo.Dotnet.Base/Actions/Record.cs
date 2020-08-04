using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using LettuceIo.Dotnet.Core;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LettuceIo.Dotnet.Base.Actions
{
    public class Record : IAction
    {
        private readonly IModel _channel;
        public IObservable<ActionStats> Stats { get; set; }

        public Record(JToken settings)
        {
            _channel = settings.Value<ConnectionFactory>().CreateConnection().CreateModel();
            
        }

        private string? _consumerTag = null;
        private IDisposable? _subscription = null;
        private readonly JToken _limit = new JObject();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private string queueName = "";


        public void Start()
        {
            var consumer = new EventingBasicConsumer(_channel);
            _subscription = Observable.FromEventPattern<BasicDeliverEventArgs>(handler => consumer.Received += handler,
                handler => consumer.Received -= handler)
                .TimeInterval()
                .Limit(_limit)
                .Subscribe(OnMessage,Stop);
            _stopwatch.Restart();
            _consumerTag = _channel.BasicConsume(consumer, queueName, true, exclusive: true);
        }
        
        public void Stop()
        {
            _subscription?.Dispose();
            _channel.BasicCancel(_consumerTag);
        }

        private void OnMessage(TimeInterval<EventPattern<BasicDeliverEventArgs>> obj)
        {
            throw new NotImplementedException();
        }

        
    }
}