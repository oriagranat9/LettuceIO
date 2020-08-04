using System;

namespace LettuceIo.Dotnet.Core
{
    public struct Message
    {
        public string RoutingKey;
        public byte[] Body;
        public TimeSpan TimeDelta;
    }
}