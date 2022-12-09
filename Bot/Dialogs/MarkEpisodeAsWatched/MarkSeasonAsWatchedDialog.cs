using Bot.Models;
using Bot.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using MockSeries;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class MarkSeasonAsWatchedDialog : ComponentDialog
    {
        private readonly SeriesClient _seriesClient;

        public MarkSeasonAsWatchedDialog(SeriesClient seriesClient)
        {
            _seriesClient = seriesClient;
            var waterfallSteps = new WaterfallStep[]
            {
                CheckConfirmation,
                MarkSeasonDialog
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(MarkSeasonAsWatchedDialog), waterfallSteps));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt) + nameof(MarkAsWatchedRootDialog), DialogHelper.InitializeConfirmChoiceDictionary(), null, Common.Spanish));
            InitialDialogId = nameof(WaterfallDialog) + nameof(MarkSeasonAsWatchedDialog);
        }

        private async Task<DialogTurnResult> CheckConfirmation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO = stepContext.Options as MarkEpisodeAsWatchDTO;
            stepContext.Values.Add(nameof(MarkEpisodeAsWatchDTO), markEpisodeAsWatchDTO);
            PromptOptions promptOptions = new PromptOptions()
            {

                Prompt = MessageFactory.Text(Resources.MarkEpisodeAsWhatched.MarkEpisodeAsWatchDTO_CheckConfirmation_PromptSeason(markEpisodeAsWatchDTO.Season, markEpisodeAsWatchDTO.SeriesName)),
                RetryPrompt = MessageFactory.Text(Common.CheckConfirmation_RetryPrompt),
                Style = ListStyle.SuggestedAction,
            };
            return await stepContext.PromptAsync(nameof(ConfirmPrompt) + nameof(MarkAsWatchedRootDialog), promptOptions, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> MarkSeasonDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is bool confirmation
                && confirmation)
            {
                MarkEpisodeAsWatchDTO dto = stepContext.Values[nameof(MarkEpisodeAsWatchDTO)] as MarkEpisodeAsWatchDTO;
                if (await _seriesClient.MarkEpisodeAsWatch(stepContext.Context.Activity.From.Id, dto.SeriesName, dto.Season, dto.Episode))
                {
                    await stepContext.Context.SendActivityAsync(Resources.MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_MarkEpisode_EnterSeason(dto.Season, dto.SeriesName), cancellationToken: cancellationToken);
                    return await stepContext.EndDialogAsync(confirmation, cancellationToken: cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_MarkEpisode_Skip, cancellationToken: cancellationToken);
            }
            return await stepContext.EndDialogAsync(false, cancellationToken: cancellationToken);
        }
    }
}
