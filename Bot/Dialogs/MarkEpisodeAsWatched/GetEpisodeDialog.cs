using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Text.Json;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class GetEpisodeDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        public GetEpisodeDialog(ConversationState conversationState) : base(nameof(GetEpisodeDialog))
        {
            _conversationState = conversationState;
            var waterfallSteps = new WaterfallStep[]
          {
                GetEpisodeFromCLU,
                AskForEpisode,
                ConfirmationEpisode
          };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(GetEpisodeDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt) + nameof(GetEpisodeDialog)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetEpisodeDialog);
        }

        private async Task<DialogTurnResult> GetEpisodeFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var dto = stepContext.Options as MarkEpisodeAsWatchDTO;
            IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            JsonElement entitiesJson = CLUPrediction.GetProperty("entities");

            JsonElement[] entities = entitiesJson.Deserialize<JsonElement[]>();

            int episode = -1;
            bool success = false;
            foreach (JsonElement entity in entities)
            {
                if(entity.TryGetEpisodeFromEntities(ref episode))
                { 
                    dto.Episode = episode;
                    success = true;
                    break;
                }
                else if (entity.GetProperty("category").GetString() is string categoryS && categoryS.Equals("Episode"))
                {
                    dto.Episodes.Add(entity);
                    success = true;
                }
            }
            if (success) return await stepContext.EndDialogAsync(dto, cancellationToken);
            return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
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
            MarkEpisodeAsWatchDTO dto = default;
            if (stepContext.Result is string episode
               && !string.IsNullOrEmpty(episode))
            {
                dto = stepContext.Options as MarkEpisodeAsWatchDTO ?? new MarkEpisodeAsWatchDTO();
                dto.Episode = int.Parse(episode);
            }
            return await stepContext.EndDialogAsync(dto, cancellationToken);
        }
    }
}
