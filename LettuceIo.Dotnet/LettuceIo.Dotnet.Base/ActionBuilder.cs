using System;
using LettuceIo.Dotnet.Base.Actions;
using LettuceIo.Dotnet.Core;
using Newtonsoft.Json.Linq;

namespace LettuceIo.Dotnet.Base
{
    public class ActionBuilder
    {
        public string ActionType { get; set; }
        public object ConnectionDetails { get; set; }
        
        public ActionBuilder FromSettings(JObject settings)
        {
            ActionType = settings.Value<string>("ActionType");
            return this;
        }
        
        public IAction Build()
        {
            if(ActionType == "Record")
                return new Record();
            throw new ArgumentException($"No such actionType: {ActionType}",nameof(ActionType));
        }
    }
}