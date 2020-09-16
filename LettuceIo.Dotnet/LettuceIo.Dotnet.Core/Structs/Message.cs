using System;

namespace LettuceIo.Dotnet.Core.Structs
{
    public struct Message
    {
        public string RoutingKey;
        public byte[] Body;
        public TimeSpan TimeDelta;
    }
}