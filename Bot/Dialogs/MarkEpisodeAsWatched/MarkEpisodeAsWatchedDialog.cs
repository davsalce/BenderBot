using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Prompts;
using MockSeries;
using MockSeries.Models;
using System.Text.Json;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class MarkEpisodeAsWatchedDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        private readonly SeriesClient _seriesClient;
        private readonly GetSeriesNameDialog _seriesNameDialog;
        private readonly GetSeasonDialog _seasonDialog;
        private readonly GetEpisodeDialog _episodeDialog;
        public MarkEpisodeAsWatchedDialog(ConversationState conversationState, SeriesClient seriesClient, GetSeriesNameDialog seriesNameDialog, GetSeasonDialog seasonDialog, GetEpisodeDialog episodeDialog)
        {
            _conversationState = conversationState;
            _seriesClient = seriesClient;
            _seriesNameDialog = seriesNameDialog;
            _seasonDialog = seasonDialog;
            _episodeDialog = episodeDialog;
            var waterfallSteps = new WaterfallStep[]
            {
                SetCLUFlag,
                GetSeriesNameFromCLU,
                StoreSeriesName,
                GetSeasonAndEpisodeFromCLU,
                GetLastOrFirstUnwatchedEpisode,
                GetSeason,
                StoreSeason,
                GetEpisode,
                StoreEpisode,
                CheckConfirmation,
                MarkEpisode,
                CleanCLUFlag,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(MarkEpisodeAsWatchedDialog), waterfallSteps));

            var culture = new PromptCultureModel()
            {
                InlineOr = " o ",
                InlineOrMore = "",
                Locale = "es-es",
                Separator = ",",
                NoInLanguage = "No",
                YesInLanguage = "Sí",
            };

            var customDictionary = new Dictionary<string, (Choice, Choice, ChoiceFactoryOptions)>()
            {
                    { culture.Locale,
                                        (
                                        new Choice(culture.YesInLanguage),
                                        new Choice(culture.NoInLanguage),
                                        new ChoiceFactoryOptions(culture.Separator,culture.InlineOr, culture.InlineOrMore, true)
                                        )
                    }
            };

            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt) + nameof(MarkEpisodeAsWatchedDialog), customDictionary, null, "es-ES"));
            AddDialog(_seriesNameDialog);
            AddDialog(_seasonDialog);
            AddDialog(_episodeDialog);
        }

        private async Task<DialogTurnResult> SetCLUFlag(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IStatePropertyAccessor<bool> CLUFlagStatePropertyAccessor = _conversationState.CreateProperty<bool>("CLUFlag");
            await CLUFlagStatePropertyAccessor.SetAsync(stepContext.Context, true, cancellationToken: cancellationToken);
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetSeriesNameFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MarkEpisodeAsWatchDTO dto = new MarkEpisodeAsWatchDTO();
            IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            JsonElement[] entities = CLUPrediction.GetEntitiesFromCLU();

            foreach (JsonElement entity in entities)
            {
                entity.GetSeriesNameFromEntities(dto);
            }

            if (string.IsNullOrEmpty(dto.SeriesName))
            {
                return await stepContext.BeginDialogAsync(_seriesNameDialog.Id, dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> StoreSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
                && !string.IsNullOrEmpty(dto.SeriesName))
            {
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetSeasonAndEpisodeFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto)
            {
                IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
                JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);
                JsonElement[] entities = CLUPrediction.GetEntitiesFromCLU();

                foreach (JsonElement entity in entities)
                {
                    if (!entity.TryGetSeasonEpisodeFromEntities(dto))
                    {
                        if (entity.GetProperty("category").GetString() is string categoryE
                            && categoryE.Equals("Episode"))
                        {
                            dto.Episodes.Add(entity);
                        }
                        else if (entity.GetProperty("category").GetString() is string categoryS
                            && categoryS.Equals("Season"))
                        {
                            dto.Seasons.Add(entity);
                        }
                    }
                }
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetLastOrFirstUnwatchedEpisode(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
                && (!dto.IsCompleteEpisode() || !dto.IsCompleteSeason()))
            {
                IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
                JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);
                JsonElement[] entities = CLUPrediction.GetEntitiesFromCLU();

                foreach (JsonElement entity in entities)
                {
                    if (entity.TryGetFirstOrLastUnwatchedEpisode())
                    {
                        dto.Season = _seriesClient.GetLastOrFirstUnwatchedEpisode(stepContext.Context.Activity.From.Id, dto.SeriesName).season;
                        dto.Episode = _seriesClient.GetLastOrFirstUnwatchedEpisode(stepContext.Context.Activity.From.Id, dto.SeriesName).episode;
                    }
                }
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetSeason(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
                && !dto.IsCompleteSeason())
            {
                return await stepContext.BeginDialogAsync(_seasonDialog.Id, dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> StoreSeason(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto)
            {
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetEpisode(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
                && !dto.IsCompleteEpisode())
            {
                return await stepContext.BeginDialogAsync(_episodeDialog.Id, dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> StoreEpisode(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto)
            {
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> CheckConfirmation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO
               && markEpisodeAsWatchDTO.IsComplete())
            {
                stepContext.Values.Add(nameof(MarkEpisodeAsWatchDTO), markEpisodeAsWatchDTO);
                PromptOptions promptOptions = new PromptOptions()
                {
                    Prompt = MessageFactory.Text($"¿Te refieres a la temporada {markEpisodeAsWatchDTO.Season} capítulo {markEpisodeAsWatchDTO.Episode} de {markEpisodeAsWatchDTO.SeriesName}?"),
                    RetryPrompt = MessageFactory.Text("Responde sí o no."),
                    Style = ListStyle.SuggestedAction,
                };
                return await stepContext.PromptAsync(nameof(ConfirmPrompt) + nameof(MarkEpisodeAsWatchedDialog), promptOptions, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> MarkEpisode(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is bool confirmation
                && confirmation)
            {
                MarkEpisodeAsWatchDTO dto = stepContext.Values[nameof(MarkEpisodeAsWatchDTO)] as MarkEpisodeAsWatchDTO;
                if (await _seriesClient.MarkEpisodeAsWatch(stepContext.Context.Activity.From.Id, dto.SeriesName, dto.Season, dto.Episode))
                {
                    await stepContext.Context.SendActivityAsync($"{dto.Season}x{dto.Episode.ToString("D2")} de {dto.SeriesName} visto", cancellationToken: cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"Mucho texto. Vete a marcarlo a la web de TrackSeries", cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> CleanCLUFlag(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IStatePropertyAccessor<bool> CLUFlagStatePropertyAccessor = _conversationState.CreateProperty<bool>("CLUFlag");
            await CLUFlagStatePropertyAccessor.SetAsync(stepContext.Context, false, cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

    }
}