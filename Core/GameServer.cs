using System;
using BomberServer.Models;

namespace BomberServer.Core
{
    public class GameServer
    {
        public MatchManager MatchManager { get; } = new();
        public GameLoop Loop { get; private set; } = default!;

        public void Start()
        {
            Console.WriteLine("[GameServer] Starting...");

            // tạo loop
            Loop = new GameLoop(MatchManager, tickRate: 10);

            // tạo match test
            var map = CreateTestMap();
            var match = MatchManager.CreateMatch(map);

            match.AddPlayer(new Player(1, 1, 1, "P1"));
            match.AddPlayer(new Player(2, 5, 1, "P2"));

            // start game loop
            Loop.Start();
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
