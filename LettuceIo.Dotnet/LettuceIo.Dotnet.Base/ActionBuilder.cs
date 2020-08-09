using System;
using LettuceIo.Dotnet.Base.Actions;
using LettuceIo.Dotnet.Core;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;

namespace LettuceIo.Dotnet.Base
{
    public class ActionBuilder
    {
        public string ActionType;
        public object ConnectionDetails;
        public IConnectionFactory ConnectionFactory;
        public Limits Limits;
        public string Queue;

        public ActionBuilder FromSettings(JToken settings)
        {
            ActionType = settings.Value<string>("ActionType");
            return this;
        }

        public IAction Build()
        {
            if (ActionType == "Record")
                return new Record(ConnectionFactory, Limits, Queue);
            throw new ArgumentException($"No such actionType: {ActionType}", nameof(ActionType));
        }
    }
}