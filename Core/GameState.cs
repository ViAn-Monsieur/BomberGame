using System.Collections.Generic;

namespace BomberServer.Core
{
    public class GameState
    {
        public int MatchId { get; set; }
        public int Tick { get; set; }

        public List<PlayerState> Players { get; set; } = new();
        public List<BombState> Bombs { get; set; } = new();
        public List<ExplosionCellState> Explosions { get; set; } = new();
    }

    public class PlayerState
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Alive { get; set; }
    }

    public class BombState
    {
        public int OwnerId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public float RemainingTime { get; set; }
        public int Power { get; set; }
    }

    public class ExplosionCellState
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
