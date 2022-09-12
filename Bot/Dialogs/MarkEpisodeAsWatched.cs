using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using MockSeries;
using System.Text.Json;

namespace Bot.Dialogs
{
    public class MarkEpisodeAsWatched : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        private readonly SeriesClient _seriesClient;
        public MarkEpisodeAsWatched(ConversationState conversationState, SeriesClient seriesClient)
        {
            _conversationState = conversationState;
            _seriesClient = seriesClient;
            var waterfallSteps = new WaterfallStep[]
            {
            GetSeasonAndChapterFromCLU,
            AskForSeasonChapterAndSerieStepAsync,
            CheckSerie,
            CheckSeason,
            CheckChapter
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(MarkEpisodeAsWatched), waterfallSteps));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
        }



        private async Task<DialogTurnResult> GetSeasonAndChapterFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Crea propieddad para acceder al json que me ha dado el CLU. Este JSON está guardado en el conversationState
            IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");

            // recuperamos el JSON de CLUPredicction
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            JsonElement entitiesJson = CLUPrediction.GetProperty("entities");

            JsonElement[] entities = JsonSerializer.Deserialize<JsonElement[]>(entitiesJson);

            MarkEpisodeAsWatchDTO dto = new MarkEpisodeAsWatchDTO();

            foreach (JsonElement entity in entities)
            {
                //guardamos nombre serie
                TryGetSeriesNameFromEntities(entity, dto);

                if (!TryGetSeasonEpisodeFromEntities(entity, dto))
                {
                    //guardamos capitulo
                    if (entity.GetProperty("category").GetString() is string categoryE && categoryE.Equals("Episode"))
                    {
                        dto.Episodes.Add(entity);
                    }
                    if (entity.GetProperty("category").GetString() is string categoryS && categoryS.Equals("Season"))
                    {
                        dto.Seasons.Add(entity);
                    }
                }
            }
            return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
        }
        private static async Task<DialogTurnResult> AskForSeasonChapterAndSerieStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MarkEpisodeAsWatchDTO dto;
            if (stepContext.Result is MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO
                && markEpisodeAsWatchDTO.Episode is not null
                && markEpisodeAsWatchDTO.Season is not null
                && markEpisodeAsWatchDTO.SeriesName is not null)
            {
                dto = markEpisodeAsWatchDTO;
                await stepContext.Context.SendActivityAsync(
                    $"¿Te refieres al capítulo {dto.Episode} de la temporada {dto.Season} de {dto.SeriesName}?",
                    cancellationToken: cancellationToken);

            }


            //preguntamos si la temporada y el capitulo cogido son correctos para la serie x si es correcto marcamos y salimos
            //si no es correcto seguimos cascada.

            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }
        private Task<DialogTurnResult> CheckSerie(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        private Task<DialogTurnResult> CheckSeason(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private Task<DialogTurnResult> CheckChapter(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private bool? TryGetSeriesNameFromEntities(JsonElement entity, MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO)
        {
            if (entity.GetProperty("category").GetString() is string category && category.Equals("Serie"))
            {
                markEpisodeAsWatchDTO.SeriesName = entity.GetProperty("text").ToString();
                return true;
            }
            return false;
        }

        //TODO


        private bool TryGetSeasonEpisodeFromEntities(JsonElement entity, MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO)
        {

            if (entity.GetProperty("category").GetString() is string category && category.Equals("SeasonEpisode"))
            {
                string? seasonEpisodeStr = entity.GetProperty("text").GetString();
                string[]? seasonEpisodeArray = seasonEpisodeStr?.Split("x");
                return TryGetSeasonEpisodeArray(seasonEpisodeArray, markEpisodeAsWatchDTO);
            }
            return default;
        }
        private bool TryGetSeasonEpisodeArray(string[]? seasonEpisodeArray, MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO)
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