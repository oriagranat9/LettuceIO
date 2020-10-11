using System;

namespace LettuceIo.Dotnet.Core.Structs
{
    public struct Metrics
    {
        public int Count;
        public TimeSpan Duration;
        public double SizeKB;
    }
}