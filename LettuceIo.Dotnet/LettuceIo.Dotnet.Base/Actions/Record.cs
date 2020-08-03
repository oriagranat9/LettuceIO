using System;
using LettuceIo.Dotnet.Core;

namespace LettuceIo.Dotnet.Base.Actions
{
    public class Record : IAction
    {
        public string ID { get; set; }
        public event Action<object> Updates;
        public bool IsRunning()
        {
            throw new NotImplementedException();
        }

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