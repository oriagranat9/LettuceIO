using System;

namespace LettuceIo.Dotnet.Core.Structs
{
    public struct Limits
    {
        public TimeSpan? Duration;
        public long? Amount;
        public double? SizeKB;
    }
}