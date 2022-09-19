﻿using Bot.CLU;
using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Prompts;
using Microsoft.Bot.Schema;
using MockSeries;
using System.Text.Json;
using static Microsoft.Bot.Builder.Dialogs.Prompts.PromptCultureModels;

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
                InicialiceMarkEpisodeAsWatchDTO,
                GetSeriesName,
                StoreSeriesName,
                GetSeason,
                StoreSeason,
                GetEpisode,
                StoreEpisode,
                CheckConfirmation,

                MarkEpisode
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
            AddDialog(_episodeDialog);
            AddDialog(_seasonDialog);
        }

        private async Task<DialogTurnResult> InicialiceMarkEpisodeAsWatchDTO(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values.Add(nameof(MarkEpisodeAsWatchDTO), new MarkEpisodeAsWatchDTO());
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(_seriesNameDialog.Id, stepContext.Values[nameof(MarkEpisodeAsWatchDTO)], cancellationToken: cancellationToken);
        }
        private async Task<DialogTurnResult> StoreSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is string seriesName
                && stepContext.Values[nameof(MarkEpisodeAsWatchDTO)] is MarkEpisodeAsWatchDTO dto)
            {
                dto.SeriesName = seriesName;
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }
        private async Task<DialogTurnResult> GetSeason(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(_seasonDialog.Id, stepContext.Values[nameof(MarkEpisodeAsWatchDTO)], cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> StoreSeason(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto)
            {
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetEpisode(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(_episodeDialog.Id, stepContext.Values[nameof(MarkEpisodeAsWatchDTO)], cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> StoreEpisode(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO dto)
            {
                return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> CheckConfirmation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO
               && markEpisodeAsWatchDTO.IsComplete())
            {

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
            if (stepContext.Result is bool confirmation)
            {
                MarkEpisodeAsWatchDTO dto = stepContext.Values[nameof(MarkEpisodeAsWatchDTO)] as MarkEpisodeAsWatchDTO;
                if (await _seriesClient.MarkEpisodeAsWatch(stepContext.Context.Activity.From.Id, dto.SeriesName, dto.Season, dto.Episode))
                {
                    await stepContext.Context.SendActivityAsync($"{dto.Season}x{dto.Episode.ToString("D2")} de {dto.SeriesName} visto", cancellationToken: cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync($"Mucho texto. Vete a marcarlo a la web de TrackSeries", cancellationToken: cancellationToken);
                }
            }
            return await stepContext.EndDialogAsync(stepContext.Result, cancellationToken: cancellationToken);
        }
    }
}