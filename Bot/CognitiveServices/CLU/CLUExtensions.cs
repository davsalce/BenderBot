using Bot.Models;
using static Bot.CognitiveServices.CLU.CLUPrediction;

namespace Bot.CognitiveServices.CLU
{
    public static class CLUExtensions
    {

        public static string GetLanguageFromEntities(this Entity entity)
        {
            string? language = null;
            if (entity.Category.Equals("Language"))
            {
                language = entity.ExtraInformation.FirstOrDefault()?.Key;
            }
            return language;
        }

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
        public static bool TryGetLastUnwatchedEpisode(this Entity entity)
        {
            if (entity.Category.Equals("Episode"))
            {
                string relativeTo = entity.Resolutions?.FirstOrDefault().RelativeTo;
                if (relativeTo is not null && relativeTo.Equals("End") && entity.Length > 7)//6 = numero de letras de Episode
                {
                    return true;
                }
                if (relativeTo is not null && relativeTo.Equals("Current") && entity.Length > 7)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool TryGetSeason(this Entity entity)
        {
            if (entity.Category.Equals("Season"))
            {
                string relativeTo = entity.Resolutions?.FirstOrDefault().RelativeTo;
                if (relativeTo is not null && relativeTo.Equals("End") && entity.Length > 6)//6 = numero de letras de Season
                {
                    return true;
                }
                if (relativeTo is not null && relativeTo.Equals("Current") && entity.Length > 6)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
