using System;
using LettuceIo.Dotnet.Core;
using LettuceIo.Dotnet.Core.Enums;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;

namespace LettuceIo.Dotnet.Base.Extensions
{
    public static class ActionFactoryExtensions
    {
        public static ActionFactory Configure(this ActionFactory f, JToken details)
        {
            f.ActionType = Enum.Parse<ActionType>(details.Value<string>("actionType"));
            f.FolderPath = details.Value<string>("folderPath");
            f.ConfigureEntities(details["selectedOption"]);
            f.ConnectionFactory = ToConnectionFactory(details["connection"]);
            f.Limits = ToLimits(details["actionDetails"]);
            if (f.ActionType == ActionType.Publish)
            {
                f.PublishOptions = ToPublishOptions(details["actionDetails"]);
            }

            return f;
        }

        public static IConnectionFactory ToConnectionFactory(JToken details) => new ConnectionFactory
        {
            HostName = details.Value<string>("amqpHostName"),
            VirtualHost = details.Value<string>("vhost"),
            UserName = details.Value<string>("username"),
            Password = details.Value<string>("password")
        };

        public static void ConfigureEntities(this ActionFactory f, JToken details)
        {
            switch (details.Value<string>("type"))
            {
                case "Queue":
                    f.Queue = details.Value<string>("name");
                    break;
                case "Exchange":
                    f.Exchange = details.Value<string>("name");
                    f.Queue = ""; //TODO
                    break;
            }
        }

        public static Limits ToLimits(JToken details) => new Limits
        {
            Amount = details["countLimit"]!.Value<bool>("status")
                ? details["countLimit"]?.Value<long>("value")
                : null,
            SizeKB = details["sizeLimit"]!.Value<bool>("status")
                ? details["sizeLimit"]?.Value<double>("value")
                : null,
            Duration = details["timeLimit"]!.Value<bool>("status")
                ? (TimeSpan?) TimeSpan.FromSeconds(details["timeLimit"]!
                    .Value<double>("value"))
                : null
        };

        public static PublishOptions ToPublishOptions(JToken details) => new PublishOptions
        {
            Loop = details.Value<bool>("isLoop"),
            Playback = details.Value<bool>("playback"),
            Shuffle = details.Value<bool>("isShuffle"),
        };
    }
}