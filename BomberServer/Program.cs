//using Networking;
//using System;

//class Program
//{
//    static void Main(string[] args)
//    {
//        TcpServer tcp = new TcpServer(7777);
//        UdpServer udp = new UdpServer(8888);

//        tcp.Start();
//        udp.Start();

//        Console.WriteLine("Server running...");
//        Console.ReadLine();
//    }
//}
//using BomberServer.Tests;

//ModelTest.Run();
//Console.ReadKey();
//return;
//using BomberServer.Core;
//using BomberServer.Models;

//var map = new GameMap(9, 7);

//// fill empty
//for (int y = 0; y < map.Height; y++)
//    for (int x = 0; x < map.Width; x++)
//        map.SetTile(x, y, TileType.Empty);

//// wall border
//for (int x = 0; x < map.Width; x++)
//{
//    map.SetTile(x, 0, TileType.Wall);
//    map.SetTile(x, map.Height - 1, TileType.Wall);
//}
//for (int y = 0; y < map.Height; y++)
//{
//    map.SetTile(0, y, TileType.Wall);
//    map.SetTile(map.Width - 1, y, TileType.Wall);
//}

//map.SetTile(3, 3, TileType.Brick);
//map.SetTile(4, 3, TileType.Brick);
//map.SetTile(5, 3, TileType.Brick);

//var manager = new MatchManager();
//var match = manager.CreateMatch(map);

//match.AddPlayer(new Player(1, 1, 1, "P1"));
//match.AddPlayer(new Player(2, 5, 1, "P2"));

//var loop = new GameLoop(manager, tickRate: 10);

//// fake input
//match.SetPlayerInput(1, PlayerInput.Right);
//match.SetPlayerInput(2, PlayerInput.Left);

//// chạy vài tick
//for (int i = 0; i < 5; i++)
//{
//    manager.Update(0.1f);
//}

//// đặt bomb
//match.SetPlayerInput(1, PlayerInput.PlaceBomb);
//manager.Update(0.1f);

//// chạy thêm để bomb nổ
//for (int i = 0; i < 30; i++)
//{
//    manager.Update(0.1f);
//}

//Console.WriteLine("DONE");
//Console.ReadKey();
using BomberServer.Core;

var server = new GameServer();
server.Start();
