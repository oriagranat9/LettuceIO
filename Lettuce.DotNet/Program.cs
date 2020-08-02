using System;
using ElectronCgi.DotNet;

namespace Lettuce.DotNet
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
