namespace LettuceIo.Dotnet.Core.Structs
{
    public struct PublishOptions
    {
        public bool Shuffle;
        public bool Loop;
        public bool Playback;
        public RateDetails RateDetails;
        public RoutingKeyDetails RoutingKeyDetails;
    }

    public struct RoutingKeyDetails
    {
        public PublishRoutingKeyType RoutingKeyType;
        public string CustomValue;
    }

    public struct RateDetails
    {
        public double RateHz;
        public int Multiplier;
    }

    public enum PublishRoutingKeyType
    {
        Recorded,
        Random,
        Custom
    }
}