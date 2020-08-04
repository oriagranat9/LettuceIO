using System;

namespace LettuceIo.Dotnet.Core
{
    public interface IAction
    {
        public IObservable<ActionMetrics> Stats { get; }
        public void Start();
        public void Stop();
    }
}