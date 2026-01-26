using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace BomberServer.Core
{
    public class GameLoop
    {
        private readonly RoomManager _roomManager;
        private readonly GameServer _gameServer;
        private bool _running;
        private int _tick;

        public int TickRate { get; }
        public float DeltaTime => 1f / TickRate;

        public GameLoop(RoomManager roomManager, GameServer server, int tickRate = 30)
        {
            _roomManager = roomManager;
            _gameServer = server;
            TickRate = tickRate;
        }

        public void Start()
        {
            Console.WriteLine("GameLoop started");

            _running = true;

            var sw = Stopwatch.StartNew();
            long last = sw.ElapsedMilliseconds;
            double tickIntervalMs = 1000.0 / TickRate;

            while (_running)
            {
                try
                {
                    long now = sw.ElapsedMilliseconds;

                    if (now - last >= tickIntervalMs)
                    {
                        last = now;
                        Update(DeltaTime);
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("========= GAME LOOP CRASH =========");
                    Console.WriteLine(ex);
                    Console.WriteLine("=================================");
                }
            }
        }

        private void Update(float dt)
        {
            _tick++;

            foreach (var room in _roomManager.AllRooms.ToList())
            {
                try
                {
                    room.Update(dt);

                    if (room.State == RoomState.Finished)
                    {
                        _roomManager.RemoveRoom(room.RoomId);
                        continue;
                    }

                    var snapshot = room.matchLogic.BuildSnapshot(_tick, room.RoomId);

                    _gameServer.SendSnapshot(room, snapshot);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Room {room.RoomId} update crash:");
                    Console.WriteLine(ex);
                }
            }
        }

        public void Stop() => _running = false;
    }
}
