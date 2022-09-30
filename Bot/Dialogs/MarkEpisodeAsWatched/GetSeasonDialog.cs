using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Text.Json;

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
                StoreMarkEpisodeAsWatchedDTO,
                AskForSeason,
                ConfirmationSeason
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(GetSeasonDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt) + nameof(GetSeasonDialog)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetSeasonDialog);
        }
        private async Task<DialogTurnResult> StoreMarkEpisodeAsWatchedDTO(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Options is MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO)
            {
                stepContext.Values.Add(nameof(MarkEpisodeAsWatchDTO), markEpisodeAsWatchDTO);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
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
            MarkEpisodeAsWatchDTO dto = default;
            if (stepContext.Result is string season
               && !string.IsNullOrEmpty(season))
            {
                dto = stepContext.Options as MarkEpisodeAsWatchDTO ?? new MarkEpisodeAsWatchDTO();
                if (int.TryParse(season, out int seasonInt))
                {
                    dto.Season = seasonInt;
                }
            }
            return await stepContext.EndDialogAsync(dto, cancellationToken);
        }
    }
}
