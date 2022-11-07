using Bot.Models;
using MockSeries.Models;
using System.Text.Json;
using static Bot.CLU.CLUPrediction;

namespace System.Text.Json
{
    public static class CLUExtensions
    {

        public static MarkEpisodeAsWatchDTO GetSeriesNameFromEntities(this Entity entity, MarkEpisodeAsWatchDTO dto) //devuelve dto (SeriesName)
        {
            if (entity.Category.Equals("Serie"))
            {
                dto.SeriesName = entity.Text;
            }
            return dto;
        }

        public static bool TryGetSeasonEpisodeFromEntities(this Entity entity, MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO)//devuelve true  (dto Episode Season)
        {
            if (entity.Category.Equals("SeasonEpisode"))
            {
                string[]? seasonEpisodeArray = entity.Text.Split("x");
                return TryGetSeasonEpisodeArray(seasonEpisodeArray, markEpisodeAsWatchDTO);
            }
            return default;
        }

        private static bool TryGetSeasonEpisodeArray(string[]? seasonEpisodeArray, MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO)
        {
            if (seasonEpisodeArray is not null && seasonEpisodeArray.Length == 2)
            {
                if (int.TryParse(seasonEpisodeArray[0], out int season)
                    && int.TryParse(seasonEpisodeArray[1], out int episode))
                {
                    markEpisodeAsWatchDTO.Season = season;
                    markEpisodeAsWatchDTO.Episode = episode;
                    return true;
                }
            }
            return false;
        }
        public static bool TryGetFirstOrLastUnwatchedEpisode(this Entity entity)
        {
            if (entity.Category.Equals("Episode"))
            {
                string relativeTo = entity.Resolutions?.FirstOrDefault().RelativeTo;
                if (relativeTo is not null && relativeTo.Equals("Start"))
                {
                    return true;
                }
                if (relativeTo is not null && relativeTo.Equals("End"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
