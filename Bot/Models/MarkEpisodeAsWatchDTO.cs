using System.Text.Json;
using static Bot.CLU.CLUPrediction;

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

        public List<Entity> Episodes { get; set; }

        private int? _episode;
        public int? Episode
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

            return entitySelected.Resolutions.FirstOrDefault()?.Value as int?;
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
                    _season = season.Resolutions.FirstOrDefault()?.Value as int?;
                    _episode = null;
                }
                else if (season?.Length < episode?.Length)
                {
                    _episode = episode?.Resolutions.FirstOrDefault()?.Value as int?;
                    _season = null;
                }
            }
        }
    }
}