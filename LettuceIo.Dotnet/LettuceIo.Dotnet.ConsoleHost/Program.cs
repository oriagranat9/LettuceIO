using System;
using System.Collections.Concurrent;
using ElectronCgi.DotNet;
using LettuceIo.Dotnet.Base;
using LettuceIo.Dotnet.Core;
using Newtonsoft.Json.Linq;

namespace LettuceIo.Dotnet.ConsoleHost
{
    internal static class Program
    {
        private static ConcurrentDictionary<string, IAction> _activeActions = new ConcurrentDictionary<string, IAction>();
        
        private static void Main()
        {
            var connection = new ConnectionBuilder().WithLogging().Build();
            connection.On<JObject>("NewAction", NewAction);
            connection.Listen();
        }

        private static JObject NewAction(JObject settings)
        {
            var builder = new ActionBuilder().FromSettings(settings);
            var action = builder.Build();
            _activeActions.TryAdd(action.ID,action);
        }
    }
}