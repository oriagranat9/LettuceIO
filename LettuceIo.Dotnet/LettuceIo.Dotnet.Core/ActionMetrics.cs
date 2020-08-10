using System;

namespace LettuceIo.Dotnet.Core
{
    public struct ActionMetrics
    {
        public int Count;
        public TimeSpan Duration;
        public double SizeKB;
    }
}