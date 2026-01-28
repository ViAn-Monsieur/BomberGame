using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BomberServer.Models;

namespace BomberServer.Core
{
    public class Room
    {
        public int RoomId { get; }
        public RoomType Type { get; }
        public RoomState State { get; private set; } = RoomState.Waiting;

        public Dictionary<int, Player> Players { get; } = new();
        public Match matchLogic { get; }
        public GameMap Map { get; }

        public bool IsFull => Players.Count >= GetMaxPlayers();
        public Dictionary<int, List<int>> Teams { get; } = new()
        {
            { 1, new List<int>() },
            { 2, new List<int>() }
        };

        public Room(int id, RoomType type, GameMap map)
        {
            RoomId = id;
            Type = type;
            Map = map; 
            matchLogic = new Match(id, map);
        }

        public (int x, int y) GetRandomSpawn()
        {
            bool flag = false;
            var spawns = Map.FindSpawnPoint();

            if (spawns.Count == 0)
                throw new Exception("No spawn points!");
            while (!flag)
            {
                var spawn = spawns[new Random().Next(0, spawns.Count)];
                flag = true;
                foreach (var p in Players.Values)
                {
                    if (p.X == spawn.x && p.Y == spawn.y)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                    return spawn;
            }
            return spawns[0];
        }

        public void AddPlayer(Player p)
        {
            Players[p.Id] = p;
            matchLogic.AddPlayer(p);

            if (Players.Count == GetMaxPlayers())
                Start();
        }

        private void Start()
        {
            State = RoomState.Playing;
            Console.WriteLine($"[Room #{RoomId}] GAME START");
        }

        public void RemovePlayer(int playerId)
        {
            if (!Players.Remove(playerId))
                return;

            matchLogic.RemovePlayer(playerId);

            if (Players.Count == 0)
            {
                State = RoomState.Finished;
            }
        }

        public void Update(float dt)
        {
            if (State != RoomState.Playing)
                return;

            matchLogic.Update(dt);
            CheckWinCondition();
        }

        public void CheckWinCondition()
        {
            if (Type == RoomType.Solo2 || Type == RoomType.Solo4)
            {
                if (matchLogic.AlivePlayerCount() == 1)
                    EndGame();
            }
            else if (Type == RoomType.Team2v2)
            {
                if (matchLogic.AliveTeams().Count() == 1)
                    EndGame();
            }
        }

        private void EndGame()
        {
            State = RoomState.Finished;
            Console.WriteLine($"[Room #{RoomId}] GAME END");
        }

        private int GetMaxPlayers()
            => Type == RoomType.Solo2 ? 2 : 4;
    }
}
