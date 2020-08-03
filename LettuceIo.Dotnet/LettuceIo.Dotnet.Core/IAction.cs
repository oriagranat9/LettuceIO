using System;

namespace LettuceIo.Dotnet.Core
{
    public interface IAction
    {
        public string Id { get; set; }
        public Action<object> OnStatus { get; set; }
        public void Start();
        public void Stop();
    }
}