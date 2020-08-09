using System;

namespace LettuceIo.Dotnet.Core
{
    public struct Limits
    {
        public TimeSpan? Duration;
        public long? Amount;
        public double? SizeKB;
    }
}