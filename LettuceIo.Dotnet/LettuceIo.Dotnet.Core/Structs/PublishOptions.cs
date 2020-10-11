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
}