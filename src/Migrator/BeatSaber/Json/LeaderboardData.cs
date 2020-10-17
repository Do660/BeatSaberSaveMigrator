using System.Collections.Generic;

namespace Migrator.BeatSaber.Json
{
    class LeaderboardData
    {
        public string _leaderboardId { get; set; }
        public IList<Score> _scores { get; set; }
    }
}
