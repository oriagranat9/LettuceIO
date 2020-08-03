using System;

namespace LettuceIo.Dotnet.Core
{
    public interface IAction
    {
        public string Id { get; set; }
        public IObservable<ActionStats> Stats { get; set; }
        public void Start();
        public void Stop();
    }
}