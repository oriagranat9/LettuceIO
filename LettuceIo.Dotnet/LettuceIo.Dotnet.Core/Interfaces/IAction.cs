using System;
using LettuceIo.Dotnet.Core.Enums;
using LettuceIo.Dotnet.Core.Structs;

namespace LettuceIo.Dotnet.Core.Interfaces
{
    public interface IAction : IDisposable
    {
        public Status Status { get; }
        public IObservable<Metrics> Metrics { get; }
        public void Start();
        public void Stop();
    }
}