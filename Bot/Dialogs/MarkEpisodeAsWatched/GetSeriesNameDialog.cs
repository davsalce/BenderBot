using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Text.Json;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class GetSeriesNameDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        public GetSeriesNameDialog(ConversationState conversationState) : base(nameof(GetSeriesNameDialog))
        {

            _conversationState = conversationState;
            var waterfallSteps = new WaterfallStep[]
            {
                //StoreMarkEpisodeAsWatchedDTO,
                AskForSeriesName,
                ConfirmationSeriesName
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(GetSeriesNameDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt) + nameof(GetSeriesNameDialog)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetSeriesNameDialog);
        }

        //private async Task<DialogTurnResult> StoreMarkEpisodeAsWatchedDTO(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    if (stepContext.Options is MarkEpisodeAsWatchDTO markEpisodeAsWatchDTO)
        //    {
        //        stepContext.Values.Add(nameof(MarkEpisodeAsWatchDTO), markEpisodeAsWatchDTO);
        //    }
        //    return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        //}

        private async Task<DialogTurnResult> AskForSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt) + nameof(GetSeriesNameDialog), new PromptOptions
            { Prompt = MessageFactory.Text("Introduce el nombre de la serie.") }, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmationSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            MarkEpisodeAsWatchDTO dto = stepContext.Options as MarkEpisodeAsWatchDTO ?? new MarkEpisodeAsWatchDTO();
            if (stepContext.Result is string seriesName
               && !string.IsNullOrEmpty(seriesName))
            {
                dto.SeriesName = seriesName;
            }
            return await stepContext.EndDialogAsync(dto, cancellationToken);
        }
    }
}
