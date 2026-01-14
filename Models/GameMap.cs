using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BomberServer.Models
{
    /*
     TILE CODE:
     0 = Empty
     1 = Hard Wall
     2 = Soft Wall
     3 = Spawn
    */
    internal class GameMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int[,] Tiles { get; private set; }

        public List<(int x, int y)> SpawnPoints { get; private set; } = new();

        public static GameMap Load(string path)
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, path);
            if (!File.Exists(fullPath))
                throw new Exception("Map file not found: " + fullPath);

            string json = File.ReadAllText(fullPath);
            dynamic data = JsonConvert.DeserializeObject(json);

            GameMap map = new GameMap
            {
                Width = data.width,
                Height = data.height,
                Tiles = new int[(int)data.height, (int)data.width]
            };

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    int tile = data.tiles[y][x];
                    map.Tiles[y, x] = tile;

                    if (tile == 3)
                        map.SpawnPoints.Add((x, y));
                }
            }

            Console.WriteLine($"Map loaded: {map.Width}x{map.Height}");
            Console.WriteLine($"Spawn points: {map.SpawnPoints.Count}");

            return map;
        }

        public bool IsInside(int x, int y)
            => x >= 0 && y >= 0 && x < Width && y < Height;

        public bool IsWalkable(int x, int y)
            => IsInside(x, y) && (Tiles[y, x] == 0 || Tiles[y, x] == 3);

        public bool IsHardWall(int x, int y)
            => IsInside(x, y) && Tiles[y, x] == 1;

        public bool IsSoftWall(int x, int y)
            => IsInside(x, y) && Tiles[y, x] == 2;

        public void DestroySoftWall(int x, int y)
        {
            if (IsSoftWall(x, y))
                Tiles[y, x] = 0;
        }

        public void Print()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char c = Tiles[y, x] switch
                    {
                        0 => '.',
                        1 => '#',
                        2 => '*',
                        3 => 'S',
                        _ => '?'
                    };
                    Console.Write(c + " ");
                }
                Console.WriteLine();
            }
        }
        public void PrintMap(Player player)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x == player.X && y == player.Y)
                    {
                        Console.Write("P ");
                        continue;
                    }
                    char c = Tiles[y, x] switch
                    {
                        0 => '.',
                        1 => '#',
                        2 => '*',
                        3 => '.',
                        _ => '?'
                    };
                    Console.Write(c + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
