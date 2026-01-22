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

            // 1) Tạo map 9x7
            var map = new GameMap(9, 7);

            // Fill Empty
            for (int y = 0; y < map.Height; y++)
                for (int x = 0; x < map.Width; x++)
                    map.SetTile(x, y, TileType.Empty);

            // Tạo tường viền
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

            // Đặt vài brick
            map.SetTile(3, 3, TileType.Brick);
            map.SetTile(4, 3, TileType.Brick);
            map.SetTile(5, 3, TileType.Brick);

            // Spawn
            map.SetTile(1, 1, TileType.SpawnPoint);
            map.SetTile(7, 5, TileType.SpawnPoint);

            PrintMap(map);

            // 2) Tạo 2 player
            var p1 = new Player(1, 1, 1, "P1");
            var p2 = new Player(2, 7, 5, "P2");

            // bomb list
            var bombs = new List<Bomb>();

            bool IsBombAt(int x, int y)
            {
                foreach (var b in bombs)
                    if (!b.IsExploded && b.X == x && b.Y == y)
                        return true;
                return false;
            }

            // 3) Test movement (p1 đi qua phải 2 bước)
            Console.WriteLine("\n--- Test Movement ---");
            p1.SetInput(PlayerInput.Right);
            p1.UpdateMove(map, IsBombAt);
            Console.WriteLine($"P1 pos: ({p1.X},{p1.Y}) expected (2,1)");

            p1.SetInput(PlayerInput.Right);
            p1.UpdateMove(map, IsBombAt);
            Console.WriteLine($"P1 pos: ({p1.X},{p1.Y}) expected (3,1)");

            // 4) Test place bomb
            Console.WriteLine("\n--- Test Place Bomb ---");
            p1.SetInput(PlayerInput.PlaceBomb);
            if (p1.WantsPlaceBomb() && map.CanPlaceBomb(p1.X, p1.Y))
            {
                var bomb = new Bomb(p1.Id, p1.X, p1.Y, power: 2, fuseTime: 1.0f);
                bombs.Add(bomb);
                p1.CurrentBombsPlaced++;
                Console.WriteLine($"P1 placed bomb at ({bomb.X},{bomb.Y}) fuse={bomb.FuseTime}s");
            }

            // 5) Test bomb block movement (p1 cố đi lại vào ô bomb)
            Console.WriteLine("\n--- Test Bomb Blocks Movement ---");
            p1.SetInput(PlayerInput.Left); // muốn quay lại bomb cell
            // p1 đang ở (3,1) bomb tại (3,1) -> player đứng trên bomb ok
            // Nhưng giờ ta cho player đi ra rồi thử quay lại

            p1.SetInput(PlayerInput.Right);
            p1.UpdateMove(map, IsBombAt); // đi ra (4,1)
            Console.WriteLine($"P1 moved out to ({p1.X},{p1.Y}) expected (4,1)");

            p1.SetInput(PlayerInput.Left);
            p1.UpdateMove(map, IsBombAt); // quay lại (3,1) nhưng bị bomb chặn
            Console.WriteLine($"P1 try back to bomb -> ({p1.X},{p1.Y}) expected still (4,1)");

            // 6) Tick update bomb -> explode
            Console.WriteLine("\n--- Test Bomb Explode + Destroy Brick ---");
            float dt = 0.2f;
            float time = 0;

            while (bombs.Count > 0)
            {
                time += dt;

                for (int i = bombs.Count - 1; i >= 0; i--)
                {
                    var b = bombs[i];
                    bool exploded = b.Update(dt);

                    if (exploded)
                    {
                        Console.WriteLine($"[t={time:0.0}] Bomb exploded at ({b.X},{b.Y})");

                        var explosion = Explosion.Create(map, b.X, b.Y, b.Power, out var destroyed);
                        Console.WriteLine($"Explosion cells count = {explosion.Cells.Count}");
                        Console.WriteLine($"Bricks destroyed = {destroyed.Count}");

                        // Check kill player
                        if (p2.IsAlive)
                        {
                            foreach (var cell in explosion.Cells)
                            {
                                if (cell.X == p2.X && cell.Y == p2.Y)
                                {
                                    p2.Kill();
                                    Console.WriteLine("P2 got killed by explosion!");
                                }
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

        private static void PrintMap(GameMap map)
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
