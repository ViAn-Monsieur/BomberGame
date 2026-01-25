using System;
using System.IO;
using System.Net;
using BomberServer.Models;
using Networking;

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

            tcp = new TcpServer(7777, this);
            udp = new UdpServer(8888, this);

            tcp.Start();
            udp.Start();

            GameLoop = new GameLoop(RoomManager, tickRate: 10);
            GameLoop.Start();
        }

        public void OnClientConnected(ClientSession session)
        {
            Console.WriteLine($"Client {session.Id} connected");

            // var map = LoadRandomMap();
            // var room = RoomManager.JoinRandomRoom(RoomType.Solo2, map);

            var room = RoomManager.JoinRandomRoom(RoomType.Solo2, LoadRandomMap);

            var spawn = room.GetRandomSpawn();
            var player = new Player(session.Id, spawn.x, spawn.y, $"P{session.Id}");

            session.Player = player;
            session.Player.RoomId = room.RoomId;

            RoomManager.AddPlayerToRoom(room, player);
        }

        public void OnClientDisconnected(ClientSession session)
        {
            if (session.Player != null)
            {
                RoomManager.LeaveRoom(session.Player);
            }
        }

        public void OnUdpPacket(IPEndPoint ep, byte[] data)
        {
            PacketDispatcher.Dispatch(ep, data);
        }

        private GameMap LoadRandomMap()
        {
            var files = Directory.GetFiles("Maps", "*.json");
            if (files.Length == 0)
                throw new Exception("No map files");

            return GameMapLoader.LoadFromJson(
                files[Random.Shared.Next(files.Length)]
            );
        }
    }
}
