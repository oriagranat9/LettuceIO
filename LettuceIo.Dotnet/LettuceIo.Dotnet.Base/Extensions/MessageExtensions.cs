﻿using LettuceIo.Dotnet.Core.Structs;

namespace LettuceIo.Dotnet.Base.Extensions
{
    public static class MessageExtensions
    {
        public static double SizeKB(this Message message) => message.Body.Length / 1024d;
    }
}