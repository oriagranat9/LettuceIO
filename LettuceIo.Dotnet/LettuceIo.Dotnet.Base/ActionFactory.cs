using System;
using LettuceIo.Dotnet.Base.Actions;
using LettuceIo.Dotnet.Core.Enums;
using LettuceIo.Dotnet.Core.Interfaces;
using LettuceIo.Dotnet.Core.Structs;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace LettuceIo.Dotnet.Base
{
    public class ActionFactory : IActionFactory
    {
        public ActionType ActionType;
        public string? FolderPath;
        public string? Queue;
        public string? Exchange;
        public IConnectionFactory? ConnectionFactory;
        public Limits Limits = new Limits();
        public PublishOptions PublishOptions = new PublishOptions();
        public RecordOptions RecordOptions = new RecordOptions();

        public JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Error = (_, error) => error.ErrorContext.Handled = true //handles failed parsing
        };

        public IAction CreateAction() => ActionType switch
        {
            ActionType.Record => new Record(ConnectionFactory!, Limits, Exchange, Queue!, FolderPath!, RecordOptions,
                SerializerSettings),
            ActionType.Publish => new Publish(ConnectionFactory!, Limits, Exchange!, Queue, FolderPath!, PublishOptions,
                SerializerSettings),
            _ => throw new NotSupportedException($"Action \"{ActionType}\" is not supported")
        };
    }
}