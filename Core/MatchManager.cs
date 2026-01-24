using System;
using System.Collections.Generic;
using BomberServer.Models;
using System.Linq;

namespace BomberServer.Core
{
    public class MatchManager
    {
        private readonly Dictionary<int, Match> _matches = new();
        private int _nextMatchId = 1;

        public Match CreateMatch(GameMap map)
        {
            int matchId = _nextMatchId++;
            var match = new Match(matchId, map);
            _matches.Add(matchId, match);

            Console.WriteLine($"[MatchManager] Created Match #{matchId}");
            return match;
        }

        public Match? GetMatch(int matchId)
        {
            return _matches.TryGetValue(matchId, out var m) ? m : null;
        }

        public void RemoveMatch(int matchId)
        {
            if (_matches.Remove(matchId))
                Console.WriteLine($"[MatchManager] Removed Match #{matchId}");
        }

        //
        public Match GetOrCreateMatch()
        {
            if (_matches.Count == 0)
                throw new Exception("No match exists!");

            return _matches.Values.First();
        }
        //

        public void Update(float dt)
        {
            foreach (var kv in _matches)
            {
                kv.Value.Update(dt);
            }
        }
    }
}
