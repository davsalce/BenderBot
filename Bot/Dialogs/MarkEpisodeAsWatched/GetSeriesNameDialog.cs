using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Text.Json;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class GetSeriesNameDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        public GetSeriesNameDialog(ConversationState conversationState):base(nameof(GetSeriesNameDialog))
        {

            _conversationState = conversationState;
            var waterfallSteps = new WaterfallStep[]
            {
                GetSeriesNameFromCLU,
                AskForSeriesName,
                ConfirmationSeriesName
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(GetSeriesNameDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt) + nameof(GetSeriesNameDialog)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetSeriesNameDialog);
        }

        private async Task<DialogTurnResult> GetSeriesNameFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            JsonElement entitiesJson = CLUPrediction.GetProperty("entities");

            JsonElement[] entities = entitiesJson.Deserialize<JsonElement[]>();

            string seriesName = default;
            foreach (JsonElement entity in entities)
            {
                bool success = entity.TryGetSeriesNameFromEntities(ref seriesName);
                if (success) return await stepContext.EndDialogAsync(seriesName, cancellationToken);
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> AskForSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions 
            { Prompt = MessageFactory.Text("Introduce el nombre de la serie.") }, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmationSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is string seriesName
                && !string.IsNullOrEmpty(seriesName)) 
            {
                return await stepContext.EndDialogAsync(seriesName, cancellationToken);
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }
    }
}
