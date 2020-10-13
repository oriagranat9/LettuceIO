using System;
using LettuceIo.Dotnet.Base;
using LettuceIo.Dotnet.Core.Enums;
using LettuceIo.Dotnet.Core.Structs;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;

namespace LettuceIo.Dotnet.ConsoleHost
{
    public static class ActionFactoryExtensions
    {
        private const string Defaultname = "LettuceIO-";

        public static ActionFactory Configure(this ActionFactory f, JToken details)
        {
            f.FolderPath = details.Value<string>("folderPath");
            f.ActionType = Enum.Parse<ActionType>(details.Value<string>("actionType"));
            f.ConfigureEntities(details["selectedOption"]!, f.ActionType, details.Value<string>("id"));
            f.ConfigureActions(details["actionDetails"]!, f.ActionType);
            f.ConnectionFactory = ToConnectionFactory(details["connection"]!);
            f.Limits = ToLimits(details["actionDetails"]!);
            return f;
        }

        public static void ConfigureEntities(this ActionFactory f, JToken details, ActionType type, string id)
        {
            switch (details.Value<string>("type"))
            {
                case "Queue":
                    f.Queue = details.Value<string>("name");
                    if (type == ActionType.Publish) f.Exchange = ""; //Default AMQP exchange
                    break;
                case "Exchange":
                    f.Exchange = details.Value<string>("name");
                    if (type == ActionType.Record) f.Queue = Defaultname + id;
                    break;
            }
        }

        public static void ConfigureActions(this ActionFactory f, JToken details, ActionType actionType)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (actionType)
            {
                case ActionType.Publish:
                    f.PublishOptions = new PublishOptions
                    {
                        Loop = details.Value<bool>("isLoop"),
                        Playback = details.Value<bool>("playback"),
                        Shuffle = details.Value<bool>("isShuffle"),
                        RateDetails = ToRateDetails(details["rateDetails"]!),
                        RoutingKeyDetails = ToRoutingKeyDetails(details["routingKeyDetails"]!)
                    };
                    if (f.Exchange == "") //Default AMQP exchange
                    {
                        f.PublishOptions.RoutingKeyDetails.RoutingKeyType = PublishRoutingKeyType.Custom;
                        f.PublishOptions.RoutingKeyDetails.CustomValue = f.Queue;
                    }

                    break;
                case ActionType.Record:
                    f.RecordOptions = new RecordOptions
                    {
                        BindingRoutingKey = details.Value<string>("bindingRoutingKey")
                    };
                    break;
            }
        }

        public static IConnectionFactory ToConnectionFactory(JToken details) => new ConnectionFactory
        {
            HostName = details.Value<string>("amqpHostName"),
            Port = details.Value<int>("amqpPort"),
            VirtualHost = details.Value<string>("vhost"),
            UserName = details.Value<string>("username"),
            Password = details.Value<string>("password"),
        };

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

        public static RoutingKeyDetails ToRoutingKeyDetails(JToken details) => new RoutingKeyDetails
        {
            CustomValue = details.Value<string>("customValue"),
            RoutingKeyType = details.Value<bool>("isCustom")
                ? details.Value<bool>("isRandom") ? PublishRoutingKeyType.Random
                : PublishRoutingKeyType.Custom
                : PublishRoutingKeyType.Recorded
        };

        public static RateDetails ToRateDetails(JToken details) => new RateDetails
        {
            RateHz = details.Value<double>("rate"),
            Multiplier = details.Value<int>("multiplier")
        };
    }
}