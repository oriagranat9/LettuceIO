using System;

namespace LettuceIo.Dotnet.Core
{
    public interface IAction
    {
        public string ID { get; set; }
        public event Action<object> Updates;
        public bool IsRunning();
        public void Start();
        public void Stop();
    }
}