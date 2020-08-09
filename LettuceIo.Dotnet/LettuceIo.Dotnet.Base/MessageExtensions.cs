using LettuceIo.Dotnet.Core;

namespace LettuceIo.Dotnet.Base
{
    public static class MessageExtensions
    {
        public static double SizeKB(this Message message) => message.Body.Length / 1024d;
    }
}