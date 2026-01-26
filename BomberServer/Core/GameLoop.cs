using System;
using System.Diagnostics;
using System.Threading;

namespace BomberServer.Core
{
    public class GameLoop
    {
        private readonly RoomManager _roomManager;
        private bool _running;
        private readonly GameServer _gameServer;
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
            _running = true;

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
            _tick++;

            foreach (var room in _roomManager.AllRooms)
            {
                room.Update(dt);

                var snapshot =
                    room.matchLogic.BuildSnapshot(_tick, room.RoomId);

                _gameServer.SendSnapshot(room, snapshot);
            }
        }


        public void Stop() => _running = false;
    }
}
