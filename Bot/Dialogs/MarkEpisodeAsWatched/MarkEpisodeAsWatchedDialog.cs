using Bot.Resources;
using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using MockSeries;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class MarkEpisodeAsWatchedDialog : ComponentDialog
    {
        private readonly SeriesClient _seriesClient;

        public MarkEpisodeAsWatchedDialog(SeriesClient seriesClient)
        {
            _seriesClient = seriesClient;
            var waterfallSteps = new WaterfallStep[]
            {
                CheckConfirmation,
                MarkEpisodeDialog
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(MarkEpisodeAsWatchedDialog), waterfallSteps));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt) + nameof(MarkAsWatchedRootDialog), DialogHelper.InitializeConfirmChoiceDictionary(), null, Common.Spanish));
            InitialDialogId = nameof(WaterfallDialog) + nameof(MarkEpisodeAsWatchedDialog);
        }

        private async Task<DialogTurnResult> CheckConfirmation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MarkEpisodeAsWatchDTO dto = stepContext.Options as MarkEpisodeAsWatchDTO;
            PromptOptions promptOptions = new PromptOptions()
            {

                Prompt = MessageFactory.Text(MarkEpisodeAsWhatched.MarkEpisodeAsWatchDTO_CheckConfirmation_Prompt(dto.Season, dto.Episode, dto.SeriesName)),
                RetryPrompt = MessageFactory.Text(Common.CheckConfirmation_RetryPrompt),
                Style = ListStyle.SuggestedAction,
            };
            return await stepContext.PromptAsync(nameof(ConfirmPrompt) + nameof(MarkAsWatchedRootDialog), promptOptions, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> MarkEpisodeDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is bool confirmation
                 && confirmation)
            {
                MarkEpisodeAsWatchDTO dto = stepContext.Options as MarkEpisodeAsWatchDTO;
                if (await _seriesClient.MarkEpisodeAsWatch(stepContext.Context.Activity.From.Id, dto.SeriesName, dto.Season, dto.Episode))
                {
                    await stepContext.Context.SendActivityAsync(Resources.MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_MarkEpisode_Enter(dto.Season, dto.Episode, dto.SeriesName), cancellationToken: cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_MarkEpisode_Skip, cancellationToken: cancellationToken);
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
