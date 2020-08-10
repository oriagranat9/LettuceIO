using System;
using LettuceIo.Dotnet.Base.Actions;
using LettuceIo.Dotnet.Core;
using LettuceIo.Dotnet.Core.Enums;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace LettuceIo.Dotnet.Base
{
    public class ActionFactory
    {
        public ActionType ActionType;
        public string FolderPath;
        public string Queue;
        public string Exchange;
        public IConnectionFactory ConnectionFactory;
        public Limits Limits = new Limits();
        public JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

        public IAction Build() => ActionType switch
        {
            ActionType.Record => new Record(ConnectionFactory, Limits, Queue, FolderPath, SerializerSettings),
            _ => throw new NotSupportedException($"Action \"{ActionType}\" is not supported")
        };
    }
}