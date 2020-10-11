using LettuceIo.Dotnet.Core.Enums;

namespace LettuceIo.Dotnet.Core.Structs
{
    public struct RoutingKeyDetails
    {
        public PublishRoutingKeyType RoutingKeyType;
        public string CustomValue;
    }
}