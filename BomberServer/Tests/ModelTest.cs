using System;
using System.Collections.Generic;
using BomberServer.Models;

namespace BomberServer.Tests
{
    public static class ModelTest
    {
        public static void Run()
        {
            Console.WriteLine("===== MODEL TEST START =====");

            var map = new GameMap(9, 7);

            // Fill empty
            for (int y = 0; y < map.Height; y++)
                for (int x = 0; x < map.Width; x++)
                    map.SetTile(x, y, TileType.Empty);

            // Walls
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

            // Bricks
            map.SetTile(3, 3, TileType.Brick);
            map.SetTile(4, 3, TileType.Brick);
            map.SetTile(5, 3, TileType.Brick);

            // Spawn
            map.SetTile(1, 1, TileType.SpawnPoint);
            map.SetTile(7, 5, TileType.SpawnPoint);

            PrintMap(map);

            var p1 = new Player(1, 1, 1, "P1");
            var p2 = new Player(2, 7, 5, "P2");

            var bombs = new List<Bomb>();

            bool IsBombAt(int x, int y)
            {
                foreach (var b in bombs)
                    if (!b.IsExploded && b.X == x && b.Y == y)
                        return true;

                return false;
            }

            // ---------------- MOVE ----------------
            Console.WriteLine("\n--- MOVE TEST ---");

            p1.SetInput(PlayerInput.Right);
            p1.UpdateMove(map, IsBombAt);
            Console.WriteLine($"P1: {p1.X},{p1.Y} expect 2,1");

            p1.SetInput(PlayerInput.Right);
            p1.UpdateMove(map, IsBombAt);
            Console.WriteLine($"P1: {p1.X},{p1.Y} expect 3,1");

            // ---------------- PLACE BOMB ----------------
            Console.WriteLine("\n--- PLACE BOMB ---");

            p1.SetInput(PlayerInput.PlaceBomb);

            if (p1.WantsPlaceBomb() && map.CanPlaceBomb(p1.X, p1.Y))
            {
                var bomb = new Bomb(p1.Id, p1.X, p1.Y, 2, 1f);
                bombs.Add(bomb);
                p1.CurrentBombsPlaced++;

                Console.WriteLine($"Bomb placed at {bomb.X},{bomb.Y}");
            }

            // ---------------- BLOCK MOVE ----------------
            Console.WriteLine("\n--- BOMB BLOCK MOVE ---");

            p1.SetInput(PlayerInput.Right);
            p1.UpdateMove(map, IsBombAt);

            Console.WriteLine($"P1 out: {p1.X},{p1.Y}");

            p1.SetInput(PlayerInput.Left);
            p1.UpdateMove(map, IsBombAt);

            Console.WriteLine($"Try back -> {p1.X},{p1.Y} (should stay)");

            // ---------------- EXPLODE ----------------
            Console.WriteLine("\n--- EXPLOSION ---");

            float dt = 0.2f;

            while (bombs.Count > 0)
            {
                for (int i = bombs.Count - 1; i >= 0; i--)
                {
                    var b = bombs[i];

                    if (b.Update(dt))
                    {
                        Console.WriteLine($"Bomb exploded at {b.X},{b.Y}");

                        var explosion = Explosion.Create(map, b.X, b.Y, b.Power, out var destroyed);

                        Console.WriteLine($"Explosion cells: {explosion.Cells.Count}");
                        Console.WriteLine($"Bricks destroyed: {destroyed.Count}");

                        foreach (var cell in explosion.Cells)
                        {
                            if (cell.X == p2.X && cell.Y == p2.Y)
                            {
                                p2.Kill();
                                Console.WriteLine("P2 killed!");
                            }
                        }

                        bombs.RemoveAt(i);
                        p1.CurrentBombsPlaced--;
                    }
                }
            }

            PrintMap(map);

            Console.WriteLine($"P2 Alive = {p2.IsAlive}");
            Console.WriteLine("===== MODEL TEST END =====");
        }

        static void PrintMap(GameMap map)
        {
            Console.WriteLine("\nMAP:");

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    char c = map.GetTile(x, y) switch
                    {
                        TileType.Empty => '.',
                        TileType.Wall => '#',
                        TileType.Brick => '+',
                        TileType.SpawnPoint => 'S',
                        _ => '?'
                    };

                    Console.Write(c);
                }
                Console.WriteLine();
            }
        }
    }
}
