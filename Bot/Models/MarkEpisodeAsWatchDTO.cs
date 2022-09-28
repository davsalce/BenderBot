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
                    _season = GetValueFromJsonElementList(Seasons);
                }
                return _season;
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
                    _episode = GetValueFromJsonElementList(Episodes);
                }
                return _episode;
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
            return Season != int.MinValue && Episode != int.MinValue && !string.IsNullOrEmpty(SeriesName);
        }
        public bool IsCompleteSeason()
        {
            return Season != int.MinValue;
        }
        public bool IsCompleteEpisode()
        {
            return Episode != int.MinValue;
        }

        private int GetValueFromJsonElementList(List<JsonElement> list)
        {
            JsonElement selectedJsonElementSeason = list
                            .Aggregate((element, nextElement) =>
                            nextElement.GetProperty("length").GetInt32() > element.GetProperty("length").GetInt32()
                            ? nextElement
                            : element);

            JsonElement resolutionsJson = selectedJsonElementSeason.GetProperty("resolutions");
            JsonElement[] resolutions = JsonSerializer.Deserialize<JsonElement[]>(resolutionsJson);
            JsonElement seasonValue = resolutions.FirstOrDefault().GetProperty("value");

            int season = int.MinValue;
            _ = int.TryParse(seasonValue.GetRawText().Replace("\"", string.Empty), out season);
            return season;
        }

    }
}
