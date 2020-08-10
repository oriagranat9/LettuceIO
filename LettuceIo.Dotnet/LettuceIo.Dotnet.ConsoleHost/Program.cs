﻿using System;
using System.Collections.Concurrent;
using System.Net.Mime;
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
            Connection.On<JToken, string>("NewAction", NewAction);
            Connection.On<string, string>("TerminateAction", TerminateAction);
            Connection.Listen();
        }

        private static string NewAction(JToken settings)
        {
            var id = settings.Value<string>("id");
            var factory = new ActionFactory().Configure(settings);
            var action = factory.Build();
            var added = ActiveActions.TryAdd(id, action);
            if (!added) return "Failed adding action to dictionary";

            action.Stats.Subscribe(stats => Connection.Send(id, JObject.FromObject(stats)));
            action.Start();

            return "";
        }

        private static string TerminateAction(string id)
        {
            var removed = ActiveActions.TryRemove(id, out var action);
            if (removed) action.Stop();
            return removed ? "" : "Failed removing action from dictionary";
        }
    }
}