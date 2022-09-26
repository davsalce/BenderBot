using Bot.Models;
using System.Text.Json;

namespace System.Text.Json
{
    public static class CLUExtensions
    {

        public static MarkEpisodeAsWatchDTO GetSeriesNameFromEntities(this JsonElement entity, MarkEpisodeAsWatchDTO dto) //devuelve dto (SeriesName)
        { 
            if (entity.GetProperty("category").GetString() is string category && category.Equals("Serie"))
            {
                dto.SeriesName = entity.GetProperty("text").ToString();
            }
            return dto;
        }

        public static bool TryGetSeasonEpisodeFromEntities(this JsonElement entity, MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO)//devuelve true  (dto Episode Season)
        {
            if (entity.GetProperty("category").GetString() is string category && category.Equals("SeasonEpisode"))
            {
                string? seasonEpisodeStr = entity.GetProperty("text").GetString();
                string[]? seasonEpisodeArray = seasonEpisodeStr?.Split("x");
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
    }
}
