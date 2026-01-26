using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BomberServer.Models
{
    public enum TileType
    {
        Empty = 0,
        Wall = 1, //tuong cung khong pha duoc
        Brick = 2, //gach co the pha duoc
        SpawnPoint = 3 //diem spawn
    }

    public class GameMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        //tile[y,x]
        public TileType[][] Tiles { get; set; } = default!;

        public GameMap(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new TileType[height][];

            for (int y = 0; y < height; y++)
            {
                Tiles[y] = new TileType[width];
            }
        }
        //ben trong map
        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public TileType GetTile(int x, int y)
        {
            if (!IsInside(x, y)) return TileType.Wall;
            return Tiles[y][x];
        }
        public void SetTile(int x, int y, TileType type)
        {
            if (!IsInside(x, y)) return;
            Tiles[y][x] = type;
        }
        //co the di
        public bool IsWalkable(int x, int y)
        {
            TileType tile = GetTile(x, y);
            return tile == TileType.Empty || tile == TileType.SpawnPoint;
        }
        //bi chan
        public bool IsBlocked(int x, int y)
        { 
            TileType tile = GetTile(x, y);
            return tile == TileType.Wall || tile == TileType.Brick;
        }
        //co the dat bomb
        public bool CanPlaceBomb(int x, int y)
        {
            TileType tile = GetTile(x, y);
            return tile == TileType.Empty || tile == TileType.SpawnPoint;
        }
        //pha brick
        public bool DestroyBrick(int x, int y)
        {
            if (GetTile(x, y) == TileType.Brick)
            {
                SetTile(x, y, TileType.Empty);
                Console.WriteLine($"Brick at ({x}, {y}) destroyed.");
                return true;
            }
            return false;
        }
        public List<(int x, int y)> FindSpawnPoint()
        {
            var spawnPoints = new List<(int x, int y)>();
            for (int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    if (GetTile(x, y) == TileType.SpawnPoint)
                    {
                        spawnPoints.Add((x, y));
                    }
                }
            }
            return spawnPoints;
        }
    }
}
