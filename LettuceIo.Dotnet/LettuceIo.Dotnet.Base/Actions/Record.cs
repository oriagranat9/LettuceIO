using System;
using LettuceIo.Dotnet.Core;

namespace LettuceIo.Dotnet.Base.Actions
{
    public class Record : IAction
    {
        public string Id { get; set; }
        public Action<object> OnStatus { get; set; }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}