using System;
using System.Collections.Generic;
using BomberServer.Models;

namespace BomberServer.Core
{
    public class Match
    {
        public int MatchId { get; }
        public GameMap Map { get; }
        public Dictionary<int, Player> Players { get; } = new();
        public List<Bomb> Bombs { get; } = new();
        public List<Explosion> Explosions { get; } = new();

        public int Tick { get; private set; } = 0;
        public Match(int matchId, GameMap map)
        {
            MatchId = matchId;
            Map = map;
        }

        public (int x, int y) GetRandomSpawn()
        {
            var spawns = Map.FindSpawnPoint();

            if (spawns.Count == 0)
                throw new Exception("No spawn points!");

            return spawns[Random.Shared.Next(spawns.Count)];
        }

        //them player
        public void AddPlayer(Player player)
        {
            Players[player.Id] = player;
            Console.WriteLine($"[Match #{MatchId}] Player {player.Id} joined ({player.X},{player.Y})");
        }
        //xoa player
        public void RemovePlayer(int playerId)
        {
            if (Players.Remove(playerId))
                Console.WriteLine($"[Match #{MatchId}] Player {playerId} left");
        }
        public void SetPlayerInput(int playerId, PlayerInput input)
        {
            if (Players.TryGetValue(playerId, out var p))
            {
                p.SetInput(input);
            }
        }
        private bool IsBombAt(int x, int y)
        {
            foreach (var b in Bombs)
                if (!b.IsExploded && b.X == x && b.Y == y)
                    return true;
            return false;
        }
        public void Update(float deltaTime)
        {
            Tick++;
            //1. cap nhat di chuyen
            foreach (var p in Players)
            {
                p.Value.UpdateMove(Map, IsBombAt);
            }
            //2. xu ly bomb
            foreach (var p in Players)
            {
                var player = p.Value;
                if (!player.IsAlive)
                    continue;
                if (player.WantsPlaceBomb())
                {
                    TryPlaceBomb(player);
                }
            }
            //3. cap nhat bomb -> vu no
            for(int i = Bombs.Count - 1; i >= 0; i--)
            {
                var bomb = Bombs[i];
                bool exploded = bomb.Update(deltaTime);
                if (exploded)
                {
                    OnBombExplode(bomb);
                    Bombs.RemoveAt(i);
                }
            }
            //4. cap nhat vu no (life time)
            for (int i = Explosions.Count - 1; i >= 0; i--)
            {
                Explosions[i].Update(deltaTime);
                if (Explosions[i].IsExpired)
                    Explosions.RemoveAt(i);
            }
            //5.Console Player
            // foreach (var p in Players.Values)
            // {
            //     Console.WriteLine(
            //         $"Tick={Tick} Player={p.Id} Pos=({p.X},{p.Y}) Input={p.LastInput}"
            //     );
            // }
        }

        private void OnBombExplode(Bomb bomb)
        {
            //tra ve so bomb dat boi player
            if (Players.TryGetValue(bomb.OwnerId, out var owner))
            {
                owner.CurrentBombsPlaced = Math.Max(0, owner.CurrentBombsPlaced - 1);
            }
            //tao vu no + pha brick
            var explosion = Explosion.Create(Map, bomb.X, bomb.Y, bomb.Power, out var bricksDestroyed);
            Explosions.Add(explosion);
            //kill player trong vung no
            foreach (var p in Players)
            {
                var player = p.Value;
                if (!player.IsAlive)
                    continue;
                foreach (var cell in explosion.Cells)
                {
                    if(cell.X == player.X && cell.Y == player.Y)
                    {
                        player.Kill();
                        Console.WriteLine($"[Match #{MatchId}] Player {player.Id} died at ({player.X},{player.Y})");
                        break;
                    }
                }
            }
            Console.WriteLine($"[Match #{MatchId}] Bomb exploded ({bomb.X},{bomb.Y}) destroyed={bricksDestroyed.Count}");
        }

        private void TryPlaceBomb(Player player)
        {
            Console.WriteLine($"[DEBUG] TryPlaceBomb Player={player.Id} Pos=({player.X},{player.Y}) Input={player.LastInput}");
            //1.gioi han so bomb
            if (player.CurrentBombsPlaced >= player.MaxBombs)
                return;
            //2.chi dat bomb o o walkable
            if (!Map.CanPlaceBomb(player.X, player.Y))
                return;
            //3.khong dat chong len bomb khac
            if (IsBombAt(player.X, player.Y))
                return;
            //4.dat bomb
            var bomb = new Bomb(player.Id, player.X, player.Y, power: 2, fuseTime: 2.0f);
            Bombs.Add(bomb);
            player.CurrentBombsPlaced++;
            Console.WriteLine($"[Match #{MatchId}] Player {player.Id} placed bomb at ({bomb.X},{bomb.Y})");

            //clear cờ PlaceBomb để không đặt liên tục mỗi tick
            player.SetInput(player.LastInput & ~PlayerInput.PlaceBomb);
        }
    }
}