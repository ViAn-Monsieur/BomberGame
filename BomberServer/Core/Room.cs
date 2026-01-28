using BomberServer.Models;
using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
            var spawns = Map.FindSpawnPoint();

            if (spawns.Count == 0)
                throw new Exception("No spawn points!");

            while (true)
            {
                var spawn = spawns[Random.Shared.Next(spawns.Count)];

                if (!Map.IsWalkable(spawn.x, spawn.y))
                    continue;

                bool occupied = Players.Values.Any(p => p.X == spawn.x && p.Y == spawn.y);

                if (!occupied)
                    return spawn;
            }
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
            if (Type == RoomType.Team2v2)
            {
                if (matchLogic.AliveTeams().Count() == 1)
                    EndGameTeam();
            }
            else
            {
                if (matchLogic.AlivePlayerCount() <= 1)
                    EndGame();
            }
        }


        private void EndGame()
        {
            State = RoomState.Finished;

            int winner = GetWinner();
            if (winner != -1)
            {
                var winnerPlayer = Players[winner];
                if(TcpServer.Clients.TryGetValue(winnerPlayer.Id, out var session))
                {
                    Database.AddWin(session.User!.Id);
                    session.User.Wins++;
                }
            }
            var packet = new
            {
                type = "game_end",
                winner = winner
            };

            string json = JsonSerializer.Serialize(packet);

            foreach (var p in Players.Values)
            {
                if (TcpServer.Clients.TryGetValue(p.Id, out var session))
                {
                    session.Send(json);

                }
            }
            foreach (var p in Players.Values)
            {
                if (TcpServer.Clients.TryGetValue(p.Id, out var s))
                {
                    s.Player = null;
                }
            }

            Players.Clear();
            matchLogic.Players.Clear();

            Console.WriteLine($"[Room #{RoomId}] GAME END - Winner {winner}");
        }
        private void EndGameTeam()
        {
            State = RoomState.Finished;

            int team = GetWinnerTeam();

            var packet = new
            {
                type = "game_end",
                team = team
            };

            string json = JsonSerializer.Serialize(packet);

            foreach (var p in Players.Values)
                if (TcpServer.Clients.TryGetValue(p.Id, out var session))
                    session.Send(json);
            foreach (var p in Players.Values)
            {
                if (TcpServer.Clients.TryGetValue(p.Id, out var s))
                {
                    s.Player = null; 
                }
            }
            Players.Clear();
            matchLogic.Players.Clear();

            Console.WriteLine($"[Room #{RoomId}] GAME END - Team {team}");
        }


        private int GetMaxPlayers()
            => Type == RoomType.Solo2 ? 2 : 4;
        private int GetWinner()
        {
            var alive = Players.Values.Where(p => p.IsAlive).ToList();

            if (alive.Count == 0)
                return -1; // draw

            return alive[0].Id;
        }


        private int GetWinnerTeam()
        {
            return matchLogic.AliveTeams().First();
        }

    }
}
