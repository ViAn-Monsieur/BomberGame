using System;
using System.Collections.Generic;
using BomberServer.Models;
using System.Linq;

namespace BomberServer.Core
{
    public class Match
    {
        public int MatchId { get; }
        public GameMap Map { get; }

        // Match KHÔNG sở hữu Player, chỉ dùng reference
        public Dictionary<int, Player> Players { get; } = new();

        public List<Bomb> Bombs { get; } = new();
        public List<Explosion> Explosions { get; } = new();
        private readonly List<(int x, int y)> destroyedBricks = new();

        public Match(int matchId, GameMap map)
        {
            MatchId = matchId;
            Map = map;
        }

        public void AddPlayer(Player player)
        {
            Players[player.Id] = player;
        }

        public void RemovePlayer(int playerId)
        {
            Players.Remove(playerId);
        }

        public void Update(float deltaTime)
        {
            Console.WriteLine("MATCH UPDATE");
            foreach (var p in Players.Values)
            {
                Console.WriteLine($"CALL UPDATE MOVE {p.Id}");
                p.UpdateMove(Map, IsBombAt);
            }

            foreach (var p in Players.Values)
            {
                if (p.IsAlive && p.WantsPlaceBomb())
                    TryPlaceBomb(p);
            }

            UpdateBombs(deltaTime);
            UpdateExplosions(deltaTime);
        }

        private void UpdateBombs(float dt)
        {
            for (int i = Bombs.Count - 1; i >= 0; i--)
            {
                if (Bombs[i].Update(dt))
                {
                    Explode(Bombs[i]);
                    Bombs.RemoveAt(i);
                }
            }
        }

        private void UpdateExplosions(float dt)
        {
            for (int i = Explosions.Count - 1; i >= 0; i--)
            {
                Explosions[i].Update(dt);
                if (Explosions[i].IsExpired)
                    Explosions.RemoveAt(i);
            }
        }

        private void Explode(Bomb bomb)
        {
            if (Players.TryGetValue(bomb.OwnerId, out var owner))
                owner.CurrentBombsPlaced--;

            var explosion = Explosion.Create(Map, bomb.X, bomb.Y, bomb.Power, out var bricks);

            Explosions.Add(explosion);

            destroyedBricks.AddRange(bricks);

            foreach (var p in Players.Values.Where(p => p.IsAlive))
                if (explosion.Cells.Any(c => c.X == p.X && c.Y == p.Y))
                    p.Kill();
        }


        private bool IsBombAt(int x, int y)
            => Bombs.Any(b => !b.IsExploded && b.X == x && b.Y == y);

        private void TryPlaceBomb(Player player)
        {
            if (player.CurrentBombsPlaced >= player.MaxBombs) return;
            if (!Map.CanPlaceBomb(player.X, player.Y)) return;
            if (IsBombAt(player.X, player.Y)) return;

            Bombs.Add(new Bomb(player.Id, player.X, player.Y, 2, 2f));
            player.CurrentBombsPlaced++;
            // player.ClearPlaceBombFlag();
        }

        // === THUẦN TRẠNG THÁI ===
        public int AlivePlayerCount()
            => Players.Values.Count(p => p.IsAlive);

        public IEnumerable<int> AliveTeams()
            => Players.Values.Where(p => p.IsAlive).Select(p => p.TeamId).Distinct();
        public GameState BuildSnapshot(int tick, int roomId)
        {
            var state = new GameState();
            state.Tick = tick;
            state.RoomId = roomId;
            state.MatchId = MatchId;
            state.Map = Map;
            foreach (var p in Players.Values)
            {
                state.Players.Add(new PlayerState
                {
                    Id = p.Id,
                    NickName = p.nickName,
                    X = p.X,
                    Y = p.Y,
                    Alive = p.IsAlive
                });
            }

            foreach (var b in Bombs)
            {
                state.Bombs.Add(new BombState
                {
                    OwnerId = b.OwnerId,
                    X = b.X,
                    Y = b.Y,
                    Timer = b.FuseTime,
                    Power = b.Power
                });
            }

            foreach (var e in Explosions)
            {
                foreach (var c in e.Cells)
                {
                    state.Explosions.Add(new ExplosionCellState
                    {
                        X = c.X,
                        Y = c.Y
                    });
                }
            }

            return state;
        }
    }
}
