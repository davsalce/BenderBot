using Bot.CLU;
using Bot.Models;
using Bot.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Prompts;
using MockSeries;
using System.Text.Json;
using static Bot.CLU.CLUPrediction;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class MarkAsWatchedRootDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        private readonly SeriesClient _seriesClient;
        private readonly GetSeriesNameDialog _seriesNameDialog;
        private readonly GetSeasonDialog _seasonDialog;
        private readonly GetEpisodeDialog _episodeDialog;
        private readonly MarkSeasonAsWatchedDialog _markSeasonAsWatchedDialog;
        private readonly MarkEpisodeAsWatchedDialog _markEpisodeAsWatchedDialog;
        public MarkAsWatchedRootDialog(ConversationState conversationState, SeriesClient seriesClient, GetSeriesNameDialog seriesNameDialog,
            GetSeasonDialog seasonDialog, GetEpisodeDialog episodeDialog, MarkSeasonAsWatchedDialog markSeasonAsWatchedDialog, MarkEpisodeAsWatchedDialog markEpisodeAsWatchedDialog)
        {
            _conversationState = conversationState;
            _seriesClient = seriesClient;
            _seriesNameDialog = seriesNameDialog;
            _seasonDialog = seasonDialog;
            _episodeDialog = episodeDialog;
            _markSeasonAsWatchedDialog = markSeasonAsWatchedDialog;
            _markEpisodeAsWatchedDialog = markEpisodeAsWatchedDialog;
            var waterfallSteps = new WaterfallStep[]
            {
                SetCLUFlag,
                GetSeriesNameFromCLU,
                GetSeriesNameFromDialog,
                GetSeasonAndEpisodeFromCLU,
                GetLastUnwatchedEpisodeFromCLU,
                GetSeasonCompleteFromCLU,
                GetSeasonFromDialog,
                GetEpisodeFromDialog,
                MarkEpisodeDialog,
                CleanCLUFlag
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(MarkAsWatchedRootDialog), waterfallSteps));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt) + nameof(MarkAsWatchedRootDialog), DialogHelper.InitializeConfirmChoiceDictionary(), null, Common.Spanish));
            AddDialog(_seriesNameDialog);
            AddDialog(_seasonDialog);
            AddDialog(_episodeDialog);
            AddDialog(_markSeasonAsWatchedDialog);
            AddDialog(_markEpisodeAsWatchedDialog);
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
            return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetSeriesNameFromDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
            && string.IsNullOrEmpty(dto.SeriesName))
            {
                return await stepContext.BeginDialogAsync(_seriesNameDialog.Id, dto, cancellationToken: cancellationToken);

            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetSeasonAndEpisodeFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto)
            {
                IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");
                CLUPrediction cLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);


                foreach (Entity entity in cLUPrediction.Entities)
                {
                    if (!entity.TryGetSeasonEpisodeFromEntities(dto))
                    {
                        if (entity.Category.Equals("Episode"))
                        {
                            dto.Episodes.Add(entity);
                        }
                        else if (entity.Category.Equals("Season"))
                        {
                            dto.Seasons.Add(entity);
                        }
                    }
                }
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetLastUnwatchedEpisodeFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
                && !dto.IsCompleteSeason())
            {
                IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");//mismo caso ingles que español asique no meto ?? 
                CLUPrediction cLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

                foreach (Entity entity in cLUPrediction.Entities)
                {
                    if (entity.TryGetLastUnwatchedEpisode())
                    {
                        dto.Season = _seriesClient.GetLastOrFirstUnwatchedEpisode(stepContext.Context.Activity.From.Id, dto.SeriesName).season;
                        dto.Episode = _seriesClient.GetLastOrFirstUnwatchedEpisode(stepContext.Context.Activity.From.Id, dto.SeriesName).episode;
                        break;
                    }
                }
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }
        private async Task<DialogTurnResult> GetSeasonCompleteFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
                && (!dto.IsCompleteEpisode()))//quito dy.iscompleteseasos
            {
                IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");//mismo caso ingles que español asique no meto ?? 
                CLUPrediction cLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

                foreach (Entity entity in cLUPrediction.Entities)
                {
                    if (dto.Episode == null && dto.Season != null)
                    {
                        stepContext.Values.Add(nameof(MarkEpisodeAsWatchDTO), dto);
                        return await stepContext.BeginDialogAsync(_markSeasonAsWatchedDialog.Id, dto, cancellationToken: cancellationToken);
                    }
                    else if (entity.TryGetSeason())
                    {
                        dto.Season = _seriesClient.GetUnwatchedSeason(stepContext.Context.Activity.From.Id, dto.SeriesName);
                        stepContext.Values.Add(nameof(MarkEpisodeAsWatchDTO), dto);
                        return await stepContext.BeginDialogAsync(_markSeasonAsWatchedDialog.Id, dto, cancellationToken: cancellationToken);
                    }
                }
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }
        private async Task<DialogTurnResult> GetSeasonFromDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((stepContext.Result is MarkEpisodeAsWatchDTO dto
                && !dto.IsCompleteSeason()))
            {
                return await stepContext.BeginDialogAsync(_seasonDialog.Id, dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetEpisodeFromDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((stepContext.Result is MarkEpisodeAsWatchDTO dto
                && !dto.IsCompleteEpisode() ) || (stepContext.Result is bool confirmacion && !confirmacion))
            {
                MarkEpisodeAsWatchDTO dto1 = stepContext.Values[nameof(MarkEpisodeAsWatchDTO)] as MarkEpisodeAsWatchDTO;
                return await stepContext.BeginDialogAsync(_episodeDialog.Id, dto1, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> MarkEpisodeDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto
                 && (dto.IsCompleteEpisode() && dto.IsCompleteSeason()))
            {
                return await stepContext.BeginDialogAsync(_markEpisodeAsWatchedDialog.Id, dto, cancellationToken: cancellationToken);
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