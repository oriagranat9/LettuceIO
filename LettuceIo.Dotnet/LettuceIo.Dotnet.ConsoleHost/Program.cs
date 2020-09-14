using System;
using System.Collections.Concurrent;
using ElectronCgi.DotNet;
using LettuceIo.Dotnet.Base;
using LettuceIo.Dotnet.Base.Extensions;
using LettuceIo.Dotnet.Core;
using Newtonsoft.Json.Linq;

namespace LettuceIo.Dotnet.ConsoleHost
{
    internal static class Program
    {
        private static readonly ConcurrentDictionary<string, IAction> ActiveActions =
            new ConcurrentDictionary<string, IAction>();

        private static readonly Connection Connection = new ConnectionBuilder().Build();

        private static void Main()
        {
            Connection.On<JToken, bool>("NewAction", NewAction);
            Connection.On<string>("TerminateAction", TerminateAction);
            Connection.Listen();
        }

        private static bool NewAction(JToken settings)
        {
            var id = settings.Value<string>("id");
            if (ActiveActions.ContainsKey(id)) 
                throw new Exception($"Key \"{id}\" already exists in the dictionary");
            var factory = new ActionFactory().Configure(settings);
            var action = factory.Build();
            if (!ActiveActions.TryAdd(id, action))
                throw new Exception($"Key \"{id}\" already exists in the dictionary");
            action.Stats.Subscribe(stats => Connection.Send(id, JObject.FromObject(stats)));
            action.Start();
            return true;
        }

        private static void TerminateAction(string id)
        {
            if (!ActiveActions.TryRemove(id, out var action))
                throw new Exception("Failed removing action from dictionary");
            action.Stop();
        }
    }
}