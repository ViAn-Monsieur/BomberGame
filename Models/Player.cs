using System;

namespace BomberServer.Models
{
    internal class Player
    {
        public int Id { get; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public bool IsAlive { get; private set; } = true;

        public int MaxBombs { get; private set; } = 1;
        public int BombRange { get; private set; } = 2;

        public int ActiveBombs { get; private set; } = 0;

        // ===== CONSTRUCTOR =====
        public Player(int id, int startX, int startY)
        {
            Id = id;
            X = startX;
            Y = startY;
        }

        // ===== MOVE =====
        public bool Move(int dx, int dy, GameMap map)
        {
            if (!IsAlive) return false;

            int nx = X + dx;
            int ny = Y + dy;

            if (!map.IsWalkable(nx, ny))
                return false;

            X = nx;
            Y = ny;
            return true;
        }

        // ===== PLACE BOMB =====
        public bool CanPlaceBomb()
        {
            return IsAlive && ActiveBombs < MaxBombs;
        }

        public void OnBombPlaced()
        {
            ActiveBombs++;
        }

        public void OnBombExploded()
        {
            ActiveBombs = Math.Max(0, ActiveBombs - 1);
        }

        // ===== DAMAGE =====
        public void Kill()
        {
            IsAlive = false;
        }
    }
}
