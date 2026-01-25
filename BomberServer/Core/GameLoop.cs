using System;
using System.Diagnostics;
using System.Threading;

namespace BomberServer.Core
{
    public class GameLoop
    {
        private readonly RoomManager _roomManager;
        private bool _running;

        public int TickRate { get; }
        public float DeltaTime => 1f / TickRate;

        public GameLoop(RoomManager roomManager, int tickRate = 30)
        {
            _roomManager = roomManager;
            TickRate = tickRate;
        }

        public void Start()
        {
            _running = true;
            Console.WriteLine($"[GameLoop] Start TickRate={TickRate}");

            var sw = Stopwatch.StartNew();
            long last = sw.ElapsedMilliseconds;
            double tickIntervalMs = 1000.0 / TickRate;

            while (_running)
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
        }

        private void Update(float dt)
        {
            foreach (var room in _roomManager.AllRooms)
            {
                if (room.State == RoomState.Playing)
                {
                    room.Update(dt);
                    room.CheckWinCondition();
                }
            }
        }

        public void Stop() => _running = false;
    }
}
