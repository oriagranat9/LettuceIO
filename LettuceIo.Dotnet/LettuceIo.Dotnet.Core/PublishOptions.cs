namespace LettuceIo.Dotnet.Core
{
    public struct PublishOptions
    {
        public bool Shuffle;
        public bool Loop;
        public bool Playback;
        public double RateHz;
    }
}