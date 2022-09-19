﻿using System.Text.Json;

namespace Bot.Models
{
    public class MarkEpisodeAsWatchDTO
    {
        public List<JsonElement> Seasons { get; set; }
        private int _season;
        public int Season {
            get
            {
                if(_season != -1) return _season;
                if (Seasons.Count > 0)
                {
                    int? seasonLength1 = Seasons.ElementAt(0).GetProperty("length").GetInt32();
                    int? seasonLength2 = Seasons.ElementAt(1).GetProperty("length").GetInt32();
                    int maxLength = Math.Max(seasonLength1 ?? 0, seasonLength2 ?? 0);

                    JsonElement selectedJsonElementSeason = Seasons.FirstOrDefault(s => s.GetProperty("length").GetInt32() == maxLength);

                    JsonElement resolutionsJson = selectedJsonElementSeason.GetProperty("resolutions");
                    JsonElement[] resolutions = JsonSerializer.Deserialize<JsonElement[]>(resolutionsJson);

                    var seasonStr = resolutions.FirstOrDefault().GetProperty("value").GetString();
                    return int.Parse(seasonStr);
                }
                else
                {
                    return -1;
                }
            }
            set => _season = value;
        }
        public List<JsonElement> Episodes { get; set; }
        
        private int _episode;
        public int Episode
        {
            get
            {
                if (_episode != -1) return _episode;
                if (Episodes.Count > 0)
                {
                    int? episodeLength1 = Episodes.ElementAt(0).GetProperty("length").GetInt32();
                    int? episodeLength2 = Episodes.ElementAt(1).GetProperty("length").GetInt32();
                    int maxLength = Math.Max(episodeLength1 ?? 0, episodeLength2 ?? 0);

                    JsonElement selectedJsonElementEpisode = Episodes.FirstOrDefault(s => s.GetProperty("length").GetInt32() == maxLength);

                    JsonElement resolutionsJson = selectedJsonElementEpisode.GetProperty("resolutions");
                    JsonElement[] resolutions = JsonSerializer.Deserialize<JsonElement[]>(resolutionsJson);

                    var episodeStr = resolutions.FirstOrDefault().GetProperty("value").GetString();
                    return int.Parse(episodeStr);
                }
                else 
                {
                    return -1;
                }
            }
            set => _episode = value;
        }
        public string? SeriesName { get; set; }

        public MarkEpisodeAsWatchDTO()
        {
            Seasons = new List<JsonElement>();
            Episodes = new List<JsonElement>();
            SeriesName = null;
        }
        public bool IsComplete()
        {
            return _season != -1 && _episode != -1 && !string.IsNullOrEmpty(SeriesName);
        }
    }
}