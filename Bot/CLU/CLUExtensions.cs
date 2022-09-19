using Bot.Models;
using System.Text.Json;

namespace System.Text.Json
{
    public static class CLUExtensions
    {
        public static bool TryGetSeriesNameFromEntities(this JsonElement entity, ref string seriesName)
        {
            if (entity.GetProperty("category").GetString() is string category && category.Equals("Serie"))
            {
                seriesName = entity.GetProperty("text").ToString();
                return true;
            }
            return false;
        }

        public static bool TryGetSeasonEpisodeFromEntities(this JsonElement entity, MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO)
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

        public static bool TryGetSeasonFromEntities(this JsonElement entity,ref int season)
        {

            if (entity.GetProperty("category").GetString() is string category && category.Equals("SeasonEpisode"))
            {
                string? seasonEpisodeStr = entity.GetProperty("text").GetString();
                string[]? seasonEpisodeArray = seasonEpisodeStr?.Split("x");
                return TryGetSeasonArray(seasonEpisodeArray, ref season);
            }
            return default;
        }


        private static bool TryGetSeasonArray(string[]? seasonEpisodeArray, ref int season)
        {
            if (seasonEpisodeArray is not null && seasonEpisodeArray.Length == 2)
            {
                if (int.TryParse(seasonEpisodeArray[0], out season))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool TryGetEpisodeFromEntities(this JsonElement entity, ref int episode)
        {

            if (entity.GetProperty("category").GetString() is string category && category.Equals("SeasonEpisode"))
            {
                string? seasonEpisodeStr = entity.GetProperty("text").GetString();
                string[]? seasonEpisodeArray = seasonEpisodeStr?.Split("x");
                return TryGetEpisodeArray(seasonEpisodeArray, ref episode);
            }
            return default;
        }


        private static bool TryGetEpisodeArray(string[]? seasonEpisodeArray, ref int episode)
        {
            if (seasonEpisodeArray is not null && seasonEpisodeArray.Length == 2)
            {
                if (int.TryParse(seasonEpisodeArray[1], out episode))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
