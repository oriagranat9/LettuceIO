using System;

namespace LettuceIo.Dotnet.Core
{
    public struct ActionStats
    {
        public int Count;
        public TimeSpan Duration;
        public float Size;
    }
}