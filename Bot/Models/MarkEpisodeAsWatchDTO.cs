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
                if (Seasons.Count == 1)
                {
                    SetEpisodeSeasonFromJsonElementLists(Seasons, Episodes);
                }
                else if (Seasons.Count >= 2)
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
                if (Episodes.Count == 1)
                {
                    SetEpisodeSeasonFromJsonElementLists(Seasons, Episodes);
                }
                else if (Episodes.Count >= 2)
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
            if (list == null || list.FirstOrDefault().ValueKind == JsonValueKind.Undefined) return int.MinValue;
            int value = int.MinValue;
            JsonElement selectedJsonElementSeason = list
                            .Aggregate((element, nextElement) =>
                            nextElement.GetProperty("length").GetInt32() > element.GetProperty("length").GetInt32()
                            ? nextElement
                            : element);

            if (selectedJsonElementSeason.TryGetProperty("resolutions", out JsonElement resolutionsJson))
            {


                JsonElement[] resolutions = JsonSerializer.Deserialize<JsonElement[]>(resolutionsJson);
                JsonElement seasonValue = resolutions.FirstOrDefault().GetProperty("value");
            
            value = int.MinValue;
            _ = int.TryParse(seasonValue.GetRawText().Replace("\"", string.Empty), out value);
            }
            return value;
        }
        private void SetEpisodeSeasonFromJsonElementLists(List<JsonElement> seasonslist, List<JsonElement> episodelist)
        {
            
            if (seasonslist != default && seasonslist.Any() && seasonslist.FirstOrDefault().ValueKind != default(JsonValueKind)
                && episodelist != default && episodelist.Any() && episodelist.FirstOrDefault().ValueKind != default(JsonValueKind))
            {
                int value = int.MinValue;
                if (seasonslist.FirstOrDefault().GetProperty("length").GetInt32() > episodelist.FirstOrDefault().GetProperty("length").GetInt32())
                {
                    JsonElement resolutionsJson = seasonslist.FirstOrDefault().GetProperty("resolutions");
                    JsonElement[] resolutions = JsonSerializer.Deserialize<JsonElement[]>(resolutionsJson);
                    JsonElement seasonValue = resolutions.FirstOrDefault().GetProperty("value");
                    value = int.MinValue;
                    _episode = int.MinValue;
                    _ = int.TryParse(seasonValue.GetRawText().Replace("\"", string.Empty), out value);
                   _season = value;
                }
                else
                {
                    JsonElement resolutionsJson = episodelist.FirstOrDefault().GetProperty("resolutions");
                    JsonElement[] resolutions = JsonSerializer.Deserialize<JsonElement[]>(resolutionsJson);
                    JsonElement episodeValue = resolutions.FirstOrDefault().GetProperty("value");
                    value = int.MinValue;
                    _season = int.MinValue;
                    _ = int.TryParse(episodeValue.GetRawText().Replace("\"", string.Empty), out value);
                    _episode = value;
                }
            }
        }
    }
}