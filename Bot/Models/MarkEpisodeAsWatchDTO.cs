using System.Text.Json;
using static Bot.CognitiveServices.CLU.CLUPrediction;

namespace Bot.Models
{
    public class MarkEpisodeAsWatchDTO
    {
        public List<Entity> Seasons { get; set; }
        private int? _season;
        public int? Season
        {
            get
            {
                if (_season != null) return _season;
                if (_season == null && Seasons.Count == 1)
                {
                    SetEpisodeSeasonFromJsonElementLists(Seasons, Episodes);
                }
                else if (_season == null && Seasons.Count >= 2)
                {
                    _season = GetValueFromJsonElementList(Seasons);
                }
                return _season;
            }
            set => _season = value;
        }

        public List<Entity> Episodes { get; set; }

        private int? _episode;
        public int? Episode
        {
            get
            {
                if (_episode != null) return _episode;
                if (_episode == null && Episodes.Count == 1)
                {
                    SetEpisodeSeasonFromJsonElementLists(Seasons, Episodes);
                }
                else if (_episode == null && Episodes.Count >= 2)
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
            Seasons = new List<Entity>();
            Episodes = new List<Entity>();
            SeriesName = null;
            _episode = null;
            _season = null;
        }

        public bool IsComplete()
        {
            return Season != null && Episode != null && !string.IsNullOrEmpty(SeriesName);
        }
        public bool IsCompleteSeason()
        {
            return Season != null;
        }
        public bool IsCompleteEpisode()
        {
            return Episode != null;
        }

        private int? GetValueFromJsonElementList(List<Entity> list)
        {
            if (list is null || list.FirstOrDefault() is null) return null;

            Entity entitySelected = list.OrderByDescending(entity => entity.Length).First();

            if (int.TryParse(entitySelected.Resolutions?.FirstOrDefault()?.Value.ToString(), out int value))
            {
                return value;
            }
            return null;
        }
        private void SetEpisodeSeasonFromJsonElementLists(List<Entity> seasonslist, List<Entity> episodelist)
        {

            if (seasonslist != default && seasonslist.Any()
                && episodelist != default && episodelist.Any())
            {


                Entity? season = seasonslist.FirstOrDefault();
                Entity? episode = episodelist.FirstOrDefault();
                if (season?.Length > episode?.Length)
                {
                    if (int.TryParse(season?.Resolutions?.FirstOrDefault()?.Value.ToString(), out int value))
                    {
                        _season = value;
                        _episode = null;
                    }

                }
                else if (season?.Length < episode?.Length)
                {
                    if (int.TryParse(episode?.Resolutions?.FirstOrDefault()?.Value.ToString(), out int value))
                    {
                        _episode = value;
                        _season = null;
                    }
                }
            }
        }
    }
}