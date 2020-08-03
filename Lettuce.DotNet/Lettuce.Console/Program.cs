using System;
using ElectronCgi.DotNet;
namespace Lettuce.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new ConnectionBuilder().WithLogging().Build();

            connection.Listen();
        }
    }
}