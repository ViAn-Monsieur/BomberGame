using System;
using BomberServer.Core;

namespace BomberServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "BomberServer";

            var server = new GameServer();
            server.Start();

            Console.WriteLine("Server running...");
            Console.ReadLine(); // giữ process sống
        }
    }
}
