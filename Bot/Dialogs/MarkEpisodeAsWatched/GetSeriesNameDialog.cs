using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class GetSeriesNameDialog : ComponentDialog
    {
        public GetSeriesNameDialog() : base(nameof(GetSeriesNameDialog))
        {        
            var waterfallSteps = new WaterfallStep[]
            {
                AskForSeriesName,
                ConfirmationSeriesName
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(GetSeriesNameDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt) + nameof(GetSeriesNameDialog)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetSeriesNameDialog);
        }

        private async Task<DialogTurnResult> AskForSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt) + nameof(GetSeriesNameDialog), new PromptOptions
            { Prompt = MessageFactory.Text("Introduce el nombre de la serie.") }, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmationSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MarkEpisodeAsWatchDTO dto = stepContext.Options as MarkEpisodeAsWatchDTO;
            if (stepContext.Result is string seriesName
               && !string.IsNullOrEmpty(seriesName))
            {
                dto.SeriesName = seriesName;
            }
            return await stepContext.EndDialogAsync(dto, cancellationToken);
        }
    }
}
