using Migrator.BeatSaber.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Migrator
{
    public class SaveMigrator
    {
        /// <summary>
        /// Migrates the save file up to version 1.12.1
        /// </summary>
        /// <param name="localLeaderboardPath">Path to local leaderboard file</param>
        /// <param name="songHashDataPath">Path to song hash data file</param>
        public void Up(string localLeaderboardPath, string songHashDataPath)
        {
            LocalLeaderboard localLeaderboard = JsonSerializer.Deserialize<LocalLeaderboard>(File.ReadAllText(localLeaderboardPath));
            Dictionary<string, SongHashData> songHashDatas = JsonSerializer.Deserialize<Dictionary<string, SongHashData>>(File.ReadAllText(songHashDataPath));

            string leaderboardIdPrefix = "custom_level_";
            foreach (KeyValuePair<string, SongHashData> item in songHashDatas)
            {
                string songTitleWithId = item.Key.Substring(item.Key.LastIndexOf(@"\") + 1);

                foreach (LeaderboardData leaderboardData in localLeaderboard._leaderboardsData.Where(x => x._leaderboardId.Contains(item.Value.songHash)).ToList())
                {
                    string difficulty = leaderboardData._leaderboardId.Substring(leaderboardData._leaderboardId.LastIndexOf(item.Value.songHash) + item.Value.songHash.Length);

                    LeaderboardData newLeaderboardData = localLeaderboard._leaderboardsData.SingleOrDefault(x => x._leaderboardId.EndsWith(songTitleWithId + difficulty));

                    if (newLeaderboardData == null)
                    {
                        leaderboardData._leaderboardId = leaderboardIdPrefix + songTitleWithId + difficulty;
                    }
                    else
                    {
                        foreach (Score score in leaderboardData._scores)
                        {
                            newLeaderboardData._scores.Add(score);
                        }

                        newLeaderboardData._scores = newLeaderboardData._scores.OrderByDescending(x => x._score).ToList();

                        localLeaderboard._leaderboardsData.Remove(leaderboardData);
                    }
                }
            }

            File.WriteAllText(localLeaderboardPath, JsonSerializer.Serialize<LocalLeaderboard>(localLeaderboard));
        }
    }
}
