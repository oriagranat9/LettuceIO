using System;
using System.IO;
using LettuceIo.Dotnet.Base.Actions;
using LettuceIo.Dotnet.Core;
using LettuceIo.Dotnet.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;

namespace LettuceIo.Dotnet.Base
{
    public class ActionBuilder
    {
        public ActionType ActionType;
        public string FolderPath;
        public string Queue;
        public string Exchange;


        public IConnectionFactory ConnectionFactory;
        public Limits Limits;
        public DirectoryInfo Directory;
        public JsonSerializerSettings SerializerSettings;

        public ActionBuilder Configure(JToken details)
        {
            ActionType = details.Value<ActionType>("actionType");
            FolderPath = details.Value<string>("folderPath");
            ConfigureEntities(details["selectedOption"]);
            ConfigureConnection(details["connection"]);


            return this;
        }

        private void ConfigureConnection(JToken details) =>
            ConnectionFactory = new ConnectionFactory
            {
                HostName = details.Value<string>("amqpHostName"),
                VirtualHost = details.Value<string>("vhost"),
                UserName = details.Value<string>("username"),
                Password = details.Value<string>("password")
            };

        private void ConfigureEntities(JToken details)
        {
            var type = details.Value<string>("type");
            var name = details.Value<string>("name");
            switch (type)
            {
                case "Queue":
                    Queue = name;
                    break;
                case "Exchange":
                    Exchange = name;
                    Queue = ""; //TODO
                    break;
            }
        }

        public IAction Build()
        {
            return ActionType switch
            {
                ActionType.Record => new Record(ConnectionFactory, Limits, Queue, Directory, SerializerSettings),
                _ => throw new ArgumentException($"No such actionType: {ActionType}", nameof(ActionType))
            };
        }
    }
}