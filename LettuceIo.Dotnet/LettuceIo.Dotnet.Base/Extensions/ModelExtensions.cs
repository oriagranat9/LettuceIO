using LettuceIo.Dotnet.Core.Structs;
using RabbitMQ.Client;

namespace LettuceIo.Dotnet.Base.Extensions
{
    public static class ModelExtensions
    {
        public static void BasicPublish(this IModel channel, string exchange, Message message) =>
            channel.BasicPublish(exchange, message.RoutingKey, body: message.Body);
    }
}