using LettuceIo.Dotnet.Core;

namespace LettuceIo.Dotnet.Base
{
    public class DateTimeNameProvider : INameProvider
    {
        private readonly string _format;

        public DateTimeNameProvider(string format) => _format = format;

        public string GetName() => System.DateTime.Now.ToString(_format);
    }
}