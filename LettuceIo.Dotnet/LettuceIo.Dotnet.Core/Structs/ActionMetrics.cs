using System;

namespace LettuceIo.Dotnet.Core.Structs
{
    public struct ActionMetrics
    {
        public int Count;
        public TimeSpan Duration;
        public double SizeKB;
    }
}