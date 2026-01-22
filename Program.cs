//using System;
//using BomberServer.Models;

//class Program
//{
//    static void Main(string[] args)
//    {
//        var map = GameMap.Load("D:\\C#\\GameNetworking\\BomberServer\\map01.json");
//        var player = new Player(1, map.SpawnPoints[0].x, map.SpawnPoints[0].y);
//        ConsoleKey key;
//        do
//        {
//            Console.Clear();
//            map.PrintMap(player);
//            Console.WriteLine("");
//            Console.WriteLine($"Player Position: ({player.X}, {player.Y})");
//            key = Console.ReadKey(true).Key;
//            HandleInput(key, player, map);
//        }
//        while (key != ConsoleKey.Enter);
//    }
//    static void HandleInput(ConsoleKey key, Player player, GameMap map)
//    {
//        switch (key)
//        {
//            case ConsoleKey.W:
//                player.Move(0, -1, map);
//                break;
//            case ConsoleKey.S:
//                player.Move(0, 1, map);
//                break;
//            case ConsoleKey.A:
//                player.Move(-1, 0, map);
//                break;
//            case ConsoleKey.D:
//                player.Move(1, 0, map);
//                break;
//                //case ConsoleKey.Spacebar:
//                //    //map.PlaceBomb();
//                //    break;
//        }
//    }
//}
using Networking;
using System;

class Program
{
    static void Main(string[] args)
    {
        TcpServer tcp = new TcpServer(7777);
        UdpServer udp = new UdpServer(8888);

        tcp.Start();
        udp.Start();

        Console.WriteLine("Server running...");
        Console.ReadLine();
    }
}
