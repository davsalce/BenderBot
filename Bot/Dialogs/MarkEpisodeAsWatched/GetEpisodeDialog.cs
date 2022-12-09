using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class GetEpisodeDialog : ComponentDialog
    {
        public GetEpisodeDialog() : base(nameof(GetEpisodeDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                AskForEpisode,
                ConfirmationEpisode
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(GetEpisodeDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt) + nameof(GetEpisodeDialog)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetEpisodeDialog);
        }
      
        private async Task<DialogTurnResult> AskForEpisode(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Introduce el capítulo.")
            };
            return await stepContext.PromptAsync(nameof(TextPrompt) + nameof(GetEpisodeDialog), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmationEpisode(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MarkEpisodeAsWatchDTO dto = stepContext.Options as MarkEpisodeAsWatchDTO;
            if (stepContext.Result is string episode
               && !string.IsNullOrEmpty(episode))
            {
                if (int.TryParse(episode, out int episodeInt))
                {
                    dto.Episode = episodeInt;
                }
            }
            return await stepContext.EndDialogAsync(dto, cancellationToken);
        }
    }
}
