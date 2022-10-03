using Bot.Models;
using MockSeries.Models;
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
        public static bool TryGetFirstOrLastUnwatchedEpisode(this JsonElement entity)
        {
            if (entity.GetProperty("category").GetString() is string categoryE
                && categoryE.Equals("Episode"))
            {
                if (entity.TryGetProperty("resolutions", out JsonElement resolutionsJson))
                {
                    JsonElement[] resolutions = JsonSerializer.Deserialize<JsonElement[]>(resolutionsJson);
                    if (resolutions.FirstOrDefault().TryGetProperty("relativeTo", out JsonElement episodeValue))
                    {
                        string relativeTo = episodeValue.GetString();

                        if (relativeTo is not null && relativeTo.Equals("Start"))
                        {
                            return true;
                        }
                        if (relativeTo is not null && relativeTo.Equals("End"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static JsonElement[] GetEntitiesFromCLU(this JsonElement CLUPrediction)
        {
            if ( CLUPrediction.ValueKind != JsonValueKind.Undefined && CLUPrediction.TryGetProperty("entities", out JsonElement entitiesJson)) 
            { 
                return entitiesJson.Deserialize<JsonElement[]>();
            }
            return default;
        }
    }
}
