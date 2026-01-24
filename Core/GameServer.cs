// using System;
// using BomberServer.Models;

// namespace BomberServer.Core
// {
//     public class GameServer
//     {
//         public MatchManager MatchManager { get; } = new();
//         public GameLoop Loop { get; private set; } = default!;

//         public void Start()
//         {
//             Console.WriteLine("[GameServer] Starting...");

//             // tạo loop
//             Loop = new GameLoop(MatchManager, tickRate: 10);

//             // tạo match test
//             var map = CreateTestMap();
//             var match = MatchManager.CreateMatch(map);

//             match.AddPlayer(new Player(1, 1, 1, "P1"));
//             match.AddPlayer(new Player(2, 5, 1, "P2"));

//             // start game loop
//             Loop.Start();
//         }

//         private GameMap CreateTestMap()
//         {
//             var map = new GameMap(9, 7);

//             for (int y = 0; y < map.Height; y++)
//                 for (int x = 0; x < map.Width; x++)
//                     map.SetTile(x, y, TileType.Empty);

//             for (int x = 0; x < map.Width; x++)
//             {
//                 map.SetTile(x, 0, TileType.Wall);
//                 map.SetTile(x, map.Height - 1, TileType.Wall);
//             }
//             for (int y = 0; y < map.Height; y++)
//             {
//                 map.SetTile(0, y, TileType.Wall);
//                 map.SetTile(map.Width - 1, y, TileType.Wall);
//             }

//             map.SetTile(3, 3, TileType.Brick);
//             map.SetTile(4, 3, TileType.Brick);
//             map.SetTile(5, 3, TileType.Brick);

//             map.SetTile(1, 1, TileType.SpawnPoint);
//             map.SetTile(7, 5, TileType.SpawnPoint);

//             return map;
//         }
//     }
// }


using System;
using System.Net;
using BomberServer.Models;
using Networking;

namespace BomberServer.Core
{
    public class GameServer
    {
        public MatchManager MatchManager { get; } = new();
        public GameLoop Loop { get; private set; } = default!;

        TcpServer? tcp;
        UdpServer? udp;
        
        public void Start()
        {
            Console.WriteLine("[GameServer] Starting...");

            // start network
            tcp = new TcpServer(7777, this);
            udp = new UdpServer(8888, this);

            tcp.Start();
            udp.Start();

            // tạo match
            var map = CreateTestMap();
            MatchManager.CreateMatch(map);

            // start loop
            Loop = new GameLoop(MatchManager, tickRate: 10);
            Loop.Start();
        }

        // ⭐ ĐƯỢC GỌI KHI CLIENT TCP CONNECT
        public void OnClientConnected(ClientSession session)
        {
            var match = MatchManager.GetOrCreateMatch();

            var spawn = match.GetRandomSpawn();
            var player = new Player(
                session.Id,
                spawn.x,
                spawn.y,
                $"P{session.Id}"
            );

            session.Player = player;
            match.AddPlayer(player);

            Console.WriteLine($"Player {player.nickName} joined match");
        }


        public void OnUdpPacket(IPEndPoint ep, byte[] data)
        {
            PacketDispatcher.Dispatch(ep, data);
        }

        private GameMap CreateTestMap()
        {
            var map = new GameMap(9, 7);
            for (int y = 0; y < map.Height; y++)
                for (int x = 0; x < map.Width; x++)
                    map.SetTile(x, y, TileType.Empty);

            for (int x = 0; x < map.Width; x++)
            {
                map.SetTile(x, 0, TileType.Wall);
                map.SetTile(x, map.Height - 1, TileType.Wall);
            }
            for (int y = 0; y < map.Height; y++)
            {
                map.SetTile(0, y, TileType.Wall);
                map.SetTile(map.Width - 1, y, TileType.Wall);
            }

            map.SetTile(3, 3, TileType.Brick);
            map.SetTile(4, 3, TileType.Brick);
            map.SetTile(5, 3, TileType.Brick);

            map.SetTile(1, 1, TileType.SpawnPoint);
            map.SetTile(7, 5, TileType.SpawnPoint);

            return map;
        }
    }
}
