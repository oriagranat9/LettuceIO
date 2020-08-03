using System;
using System.Threading.Tasks;
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
        public Action<object> OnStatus {  get; set; } = _ => { };
        
        public ActionStats Stats = new ActionStats();

        public Record(JToken settings)
        {
            _channel = settings.Value<ConnectionFactory>().CreateConnection().CreateModel();
        }

        public void Start()
        {
            var consumer = new EventingBasicConsumer(_channel);
            var limitReached = false;
            void ConsumerOnReceived(object? sender, BasicDeliverEventArgs e)
            {
                if(limitReached) return;
            }
            consumer.Received += ConsumerOnReceived;
            var tag = _channel.BasicConsume(consumer, "queuename", true, exclusive: true);
            _limiter.OnLimit = () =>
            {
                limitReached = true;
                _channel.BasicCancel(tag);
                consumer.Received -= ConsumerOnReceived;
            };
        }


        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}