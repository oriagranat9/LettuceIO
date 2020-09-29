namespace LettuceIo.Dotnet.Core.Structs
{
    public struct PublishOptions
    {
        public bool Shuffle;
        public bool Loop;
        public bool Playback;
        public double RateHz;
        public RoutingKeyDetails RoutingKeyDetails;
    }

    public struct RoutingKeyDetails
    {
        public PublishRoutingKeyType RoutingKeyType;
        public string CustomValue;
    }

    public enum PublishRoutingKeyType
    {
        Recorded,
        Random,
        Custom
    }
}