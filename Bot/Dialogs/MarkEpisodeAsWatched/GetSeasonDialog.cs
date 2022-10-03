using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class GetSeasonDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        public GetSeasonDialog(ConversationState conversationState) : base(nameof(GetSeasonDialog))
        {
            _conversationState = conversationState;
            var waterfallSteps = new WaterfallStep[]
            {
                AskForSeason,
                ConfirmationSeason
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(GetSeasonDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt) + nameof(GetSeasonDialog)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetSeasonDialog);
        }

        private async Task<DialogTurnResult> AskForSeason(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Introduce la temporada de la serie.")
            };
            return await stepContext.PromptAsync(nameof(TextPrompt) + nameof(GetSeasonDialog), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmationSeason(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MarkEpisodeAsWatchDTO dto = stepContext.Options as MarkEpisodeAsWatchDTO;
            if (stepContext.Result is string season
               && !string.IsNullOrEmpty(season))
            {
                if (int.TryParse(season, out int seasonInt))
                {
                    dto.Season = seasonInt;
                }
            }
            return await stepContext.EndDialogAsync(dto, cancellationToken);
        }
    }
}
