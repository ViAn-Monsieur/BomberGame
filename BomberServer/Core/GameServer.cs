using BomberServer.Models;
using Networking;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BomberServer.Core
{
    public class GameServer
    {
        public RoomManager RoomManager { get; } = new();
        public GameLoop GameLoop { get; private set; } = default!;

        TcpServer tcp = default!;
        UdpServer udp = default!;

        public void Start()
        {
            Console.WriteLine("[GameServer] Starting...");
            Database.Init();
            Console.WriteLine("[GameServer] Database initialized.");
            tcp = new TcpServer(7777, this);
            udp = new UdpServer(8888, this);

            tcp.Start();
            udp.Start();

            GameLoop = new GameLoop(RoomManager, this, 10);

            new Thread(GameLoop.Start)
            {
                IsBackground = true
            }.Start();
        }


        public void OnClientConnected(ClientSession session)
        {
            Console.WriteLine($"Client {session.Id} connected");

            session.Send(JsonSerializer.Serialize(new
            {
                type = "menu"
            }));

            Console.WriteLine("[Server] Menu sent");
        }
        //public Player CreatePlayer(ClientSession session)
        //{
        //    var p = new Player();
        //    p.Id = session.Id;
        //    p.UserId = session.User!.Id;
        //    p.NickName = session.User.NickName;

        //    session.Player = p;
        //    return p;
        //}

        public void OnClientDisconnected(ClientSession session)
        {
            if (session.Player != null)
            {
                session.Player.RemoteEndPoint = null;
                RoomManager.LeaveRoom(session.Player);
            }
        }

        public void OnUdpPacket(IPEndPoint ep, byte[] data)
        {
            try
            {
                string msg = Encoding.UTF8.GetString(data);

                using var doc = JsonDocument.Parse(msg);
                var root = doc.RootElement;

                string type = root.GetProperty("type").GetString()!;

                if (type == "udp_bind")
                {
                    int playerId = root.GetProperty("playerId").GetInt32();

                    if (!TcpServer.Clients.TryGetValue(playerId, out var session))
                        return;

                    session.BindUdp(ep);
                    if (session.Player != null)
                        session.Player.RemoteEndPoint = ep;

                }
                else if (type == "input")
                {
                    int id = root.GetProperty("playerId").GetInt32();
                    int input = root.GetProperty("input").GetInt32();

                    if (!TcpServer.Clients.TryGetValue(id, out var session))
                        return;

                    session.Player?.SetInput((PlayerInput)input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UDP PACKET ERROR:");
                Console.WriteLine(ex);
            }
        }

        private GameMap LoadRandomMap()
        {
            string mapDir = Path.Combine(AppContext.BaseDirectory, "Maps");

            Console.WriteLine("MapDir = " + mapDir);

            if (!Directory.Exists(mapDir))
                throw new Exception($"Maps folder missing: {mapDir}");

            var files = Directory.GetFiles(mapDir, "*.json");

            if (files.Length == 0)
                throw new Exception("No map files");

            // chỉ lấy file JSON thực sự bắt đầu bằng '{'
            var valid = files
                .Where(f => File.ReadAllText(f).TrimStart().StartsWith("{"))
                .ToArray();

            if (valid.Length == 0)
                throw new Exception("No valid map json");

            var chosen = valid[Random.Shared.Next(valid.Length)];

            return GameMapLoader.LoadFromJson(chosen);
        }

        public void SendSnapshot(Room room, GameState state)
        {
            if (room.Players.Count == 0)
                return;

            var bytes = JsonSerializer.SerializeToUtf8Bytes(state);

            var eps = room.Players.Values
                .Where(p => p.RemoteEndPoint != null)
                .Select(p => p.RemoteEndPoint)
                .ToList();

            if (eps.Count == 0)
                return;

            udp.SendToMany(eps, bytes);
        }

        internal void JoinRoom(ClientSession session, RoomType rt)
        {
            var room = RoomManager.JoinRandomRoom(rt, LoadRandomMap);
            var spawn = room.GetRandomSpawn();
            var player = new Player(session.Id, spawn.x, spawn.y, $"P{session.Id}");

            session.Player = player;
            player.RoomId = room.RoomId;

            room.AddPlayer(player);
            var mapPacket = new
            {
                type = "map",
                map = room.Map
            };

            string json = JsonSerializer.Serialize(mapPacket);
            session.Send(new
            {
                type = "welcome",
                playerId = player.Id,
            });
            session.Send(json);
            Console.WriteLine("[Server] Map sent");
        }
    }
}
