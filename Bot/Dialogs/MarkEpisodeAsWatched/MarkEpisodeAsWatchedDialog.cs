using Bot.CLU;
using Bot.Models;
using Bot.Resorces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Prompts;
using MockSeries;
using System.Text.Json;
using static Bot.CLU.CLUPrediction;

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
                GetSeriesNameFromDialog,
                GetSeasonAndEpisodeFromCLU,
                GetLastOrFirstUnwatchedEpisodeFromCLU,
                GetSeasonFromDialog,
                GetEpisodeFromDialog,
                CheckConfirmation,
                MarkEpisode,
                CleanCLUFlag
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(MarkEpisodeAsWatchedDialog), waterfallSteps));

            var culture = new PromptCultureModel()
            {
                InlineOr = MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_PromptCultureModel_InlineOr,
                InlineOrMore = MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_PromptCultureModel_InlineOrMore,
                Locale = Common.Locale,
                Separator = MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_PromptCultureModel_Separator,
                NoInLanguage = Common.No,
                YesInLanguage = Common.Yes
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

            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt) + nameof(MarkEpisodeAsWatchedDialog), customDictionary, null, Common.Locale));
            AddDialog(_seriesNameDialog);
            AddDialog(_seasonDialog);
            AddDialog(_episodeDialog);
        }

        private async Task<DialogTurnResult> SetCLUFlag(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IStatePropertyAccessor<bool> CLUFlagStatePropertyAccessor = _conversationState.CreateProperty<bool>("CLUFlag");//mismo caso ingles que español asique no meto ?? 
            await CLUFlagStatePropertyAccessor.SetAsync(stepContext.Context, true, cancellationToken: cancellationToken);
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetSeriesNameFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MarkEpisodeAsWatchDTO dto = new MarkEpisodeAsWatchDTO();
            
            IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");//mismo caso ingles que español asique no meto ?? 
            CLUPrediction cLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            foreach (Entity entity in cLUPrediction.Entities)
            {
                entity.GetSeriesNameFromEntities(dto);
            }

            if (string.IsNullOrEmpty(dto.SeriesName))
            {
                return await stepContext.BeginDialogAsync(_seriesNameDialog.Id, dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetSeriesNameFromDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
                IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");//mismo caso ingles que español asique no meto ?? 
                CLUPrediction cLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);
                

                foreach (Entity entity in cLUPrediction.Entities)
                {
                    if (!entity.TryGetSeasonEpisodeFromEntities(dto))
                    {
                        if (entity.Category.Equals("Episode"))//mismo caso ingles que español asique no meto ?? 
                        {
                            dto.Episodes.Add(entity);
                        }
                        else if (entity.Category.Equals("Season"))//mismo caso ingles que español asique no meto ?? 
                        {
                            dto.Seasons.Add(entity);
                        }
                    }
                }
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetLastOrFirstUnwatchedEpisodeFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
                && (!dto.IsCompleteEpisode() && !dto.IsCompleteSeason()))
            {
                IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");//mismo caso ingles que español asique no meto ?? 
                CLUPrediction cLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

                foreach (Entity entity in cLUPrediction.Entities)
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

        private async Task<DialogTurnResult> GetSeasonFromDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
                && !dto.IsCompleteSeason())
            {
                return await stepContext.BeginDialogAsync(_seasonDialog.Id, dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetEpisodeFromDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
                && !dto.IsCompleteEpisode())
            {
                return await stepContext.BeginDialogAsync(_episodeDialog.Id, dto, cancellationToken: cancellationToken);
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
                    
                    Prompt = MessageFactory.Text(MarkEpisodeAsWhatched.MarkEpisodeAsWatchDTO_CheckConfirmation_Prompt(markEpisodeAsWatchDTO.Season,markEpisodeAsWatchDTO.Episode,markEpisodeAsWatchDTO.SeriesName)),
                    RetryPrompt = MessageFactory.Text(MarkEpisodeAsWhatched.MarkEpisodeAsWatchDTO_CheckConfirmation_RetryPrompt),
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
                    await stepContext.Context.SendActivityAsync(MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_MarkEpisode_Enter(dto.Season, dto.Episode, dto.SeriesName), cancellationToken: cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_MarkEpisode_Skip, cancellationToken: cancellationToken);
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