using System;
using System.Diagnostics;
using System.Threading;

namespace BomberServer.Core
{
    public class GameLoop
    {
        private readonly MatchManager _matchManager;
        private bool _running = false;

        public int TickRate { get; }
        public float DeltaTime => 1f / TickRate;

        public GameLoop(MatchManager matchManager, int tickRate = 30)
        {
            _matchManager = matchManager;
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
                long elapsed = now - last;

                if (elapsed >= tickIntervalMs)
                {
                    last = now;
                    _matchManager.Update(DeltaTime);
                }
                else
                {
                    // ngủ 1 tí để giảm CPU
                    Thread.Sleep(1);
                }
            }
        }

        public void Stop()
        {
            _running = false;
        }
    }
}
