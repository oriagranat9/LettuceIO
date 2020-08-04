using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Timers;
using LettuceIo.Dotnet.Core;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LettuceIo.Dotnet.Base.Actions
{
    public class Record : IAction
    {
        private readonly IModel _channel;
        private readonly ILimiter _limiter;
        public string Id { get; set; }
        public IObservable<ActionStats> Stats { get; set; }

        public Record(JToken settings)
        {
            _channel = settings.Value<ConnectionFactory>().CreateConnection().CreateModel();
        }

        private string? _consumerTag = null;
        private IDisposable? _subscription = null;
        private bool _limitReached = false;
        private TimeSpan _timeLimit = TimeSpan.FromSeconds(10);
        private bool _withTimeLimit = false;
        private bool _withCountLimit = false;
        private int? _countLimit = null;

        public void Start()
        {
            var consumer = new EventingBasicConsumer(_channel);

            var received = Observable.FromEventPattern<BasicDeliverEventArgs>(handler => consumer.Received += handler,
                handler => consumer.Received -= handler);
            if (_withTimeLimit) received = received.TakeUntil(Observable.Timer(_timeLimit));
            if (_withCountLimit)
            {
                var i = 0;
                received = received.TakeWhile(pattern => i++ < _countLimit);
            }


            var timer = new Timer {AutoReset = false, Interval = _timeLimit.TotalMilliseconds};
                timer.Elapsed += (sender, args) => Stop();
                consumer.Registered += (sender, args) => timer.Start();
            }

            if (_withCountLimit)
            {
                IDisposable? unsubscribe = null;
                unsubscribe = received.Count(). ;.Subscribe(i =>
                {
                    if (i < _countLimit) return;
                    Stop();
                    unsubscribe?.Dispose();
                });
            }

            _subscription = received.TimeInterval().Subscribe(OnMessage);
            _limitReached = false;
            
            _consumerTag = _channel.BasicConsume(consumer, "queuename", true, exclusive: true);
            //start limit and stats
        }

        private void OnMessage(TimeInterval<EventPattern<BasicDeliverEventArgs>> obj)
        {
            throw new NotImplementedException();
        }


        public void Stop()
        {
            if (_consumerTag != null) _channel.BasicCancel(_consumerTag);
            _subscription?.Dispose();
        }
    }
}