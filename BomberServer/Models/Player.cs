using System;
using System.Net;

namespace BomberServer.Models
{
    [Flags]
    public enum PlayerInput
    {
        None = 0,
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        PlaceBomb = 1 << 4
    }

    public class Player
    {
        public int Id { get; }
        public string nickName { get; set; } = "";
            
        public int RoomId { get; set; }
        public int TeamId { get; set; } = 0; // 0 = solo

        //toa do
        public int X { get; private set; }
        public int Y { get; private set; }

        //trang thai nguoi choi
        public bool IsAlive { get; set; } = true;

        //thong tin ve bomb
        public int BombPower { get; set; } = 2;
        public int MaxBombs { get; set; } = 1;
        public int CurrentBombsPlaced { get; set; } = 0;

        //input state
        public PlayerInput LastInput { get; private set; } = PlayerInput.None;
        public IPEndPoint RemoteEndPoint;

        public Player(int id, int x, int y, string nickname = "")
        {
            Id = id;
            X = x;
            Y = y;
            nickName = nickname;
        }
        public void Respawn(int x, int y)
        {
            X = x;
            Y = y;
            IsAlive = true;
            CurrentBombsPlaced = 0;
            LastInput = PlayerInput.None;
        }
        public void Kill()
        {
            IsAlive = false;
            CurrentBombsPlaced = 0;
            LastInput = PlayerInput.None;
        }
        public void UpdateMove(GameMap gameMap, Func<int, int, bool> isBombAt)
        {
            Console.WriteLine($"MOVE {Id} input={LastInput}");
            if (!IsAlive)
                return;
            int dx = 0;
            int dy = 0;

            if (LastInput.HasFlag(PlayerInput.Up)) dy -= 1;
            if (LastInput.HasFlag(PlayerInput.Down)) dy += 1;
            if (LastInput.HasFlag(PlayerInput.Left)) dx -= 1;
            if (LastInput.HasFlag(PlayerInput.Right)) dx += 1;

            if(dx ==0 && dy == 0)
                return;

            int newX = X + dx;
            int newY = Y + dy;

            if (!gameMap.IsWalkable(newX, newY) || isBombAt(newX, newY))
                return;

            X = newX;
            Y = newY;
        }
        //muon dat bomb
        public bool WantsPlaceBomb()
        {
            return IsAlive && LastInput.HasFlag(PlayerInput.PlaceBomb) && CurrentBombsPlaced < MaxBombs;
        }
        public void SetInput(PlayerInput input)
        {
            LastInput = input;
        }
    }
}
