using System;

namespace LettuceIo.Dotnet.Core
{
    public interface ILimiter
    {
        public Action OnLimit { get; set; }
    }
}