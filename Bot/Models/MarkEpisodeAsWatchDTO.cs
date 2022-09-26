using System.Text.Json;

namespace Bot.Models
{
    public class MarkEpisodeAsWatchDTO
    {
        public List<JsonElement> Seasons { get; set; }
        private int _season;
        public int Season
        {
            get
            {
                if (_season != int.MinValue) return _season;
                if (Seasons.Count > 0)
                {
                    int? seasonLength1 = Seasons.ElementAt(0).GetProperty("length").GetInt32();
                    int? seasonLength2 = Seasons.ElementAt(1).GetProperty("length").GetInt32();
                    int maxLength = Math.Max(seasonLength1 ?? 0, seasonLength2 ?? 0);

                    JsonElement selectedJsonElementSeason = Seasons.FirstOrDefault(s => s.GetProperty("length").GetInt32() == maxLength);

                    JsonElement resolutionsJson = selectedJsonElementSeason.GetProperty("resolutions");
                    JsonElement[] resolutions = JsonSerializer.Deserialize<JsonElement[]>(resolutionsJson);
                    int season = int.MinValue;
                    JsonElement seasonValue = resolutions.FirstOrDefault().GetProperty("value");

                    if (int.TryParse(seasonValue.GetRawText().Replace("\"", string.Empty), out season))
                        _season = season;
                    return _season;
                }
                else
                {
                    return int.MinValue;
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
                if (_episode != int.MinValue) return _episode;
                if (Episodes.Count > 0)
                {
                    int? episodeLength1 = Episodes.ElementAt(0).GetProperty("length").GetInt32();
                    int? episodeLength2 = Episodes.ElementAt(1).GetProperty("length").GetInt32();
                    int maxLength = Math.Max(episodeLength1 ?? 0, episodeLength2 ?? 0);

                    JsonElement selectedJsonElementEpisode = Episodes.FirstOrDefault(s => s.GetProperty("length").GetInt32() == maxLength);

                    JsonElement resolutionsJson = selectedJsonElementEpisode.GetProperty("resolutions");
                    JsonElement[] resolutions = JsonSerializer.Deserialize<JsonElement[]>(resolutionsJson);


                    int episode = int.MinValue;
                    JsonElement episodeValue = resolutions.FirstOrDefault().GetProperty("value");
                    if (int.TryParse(episodeValue.GetRawText().Replace("\"", string.Empty), out episode))
                        _episode = episode;
                    return _episode;
                }
                else
                {
                    return int.MinValue;
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
            _episode = int.MinValue;
            _season = int.MinValue;
        }

        public bool IsComplete()
        {
            return _season != int.MinValue && _episode != int.MinValue && !string.IsNullOrEmpty(SeriesName);
        }
        public bool IsCompleteSeason()
        {
            return _season != int.MinValue;
        }
        public bool IsCompleteEpisode()
        {
            return _episode != int.MinValue;
        }
    }
}
