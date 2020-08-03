using System;
using ElectronCgi.DotNet;

namespace LettuceIo.Dotnet.ConsoleHost
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var connection = new ConnectionBuilder().WithLogging().Build();
            
            connection.Listen();
        }
    }
}