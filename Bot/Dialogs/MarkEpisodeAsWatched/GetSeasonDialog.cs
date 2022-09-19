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
                GetSeasonFromCLU,
                AskForSeason,
                ConfirmationSeason
          };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(GetSeasonDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt) + nameof(GetSeasonDialog)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetSeasonDialog);
        }

        private async Task<DialogTurnResult> GetSeasonFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var dto = stepContext.Options as MarkEpisodeAsWatchDTO ?? new MarkEpisodeAsWatchDTO();

            IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);
            JsonElement entitiesJson = CLUPrediction.GetProperty("entities");
            JsonElement[] entities = entitiesJson.Deserialize<JsonElement[]>();

            int season = -1;
            bool success = false;
            foreach (JsonElement entity in entities)
            {
                if (entity.TryGetSeasonFromEntities(ref season))
                {
                    dto.Season = season;
                    success = true;
                    break;
                }
                else if (entity.GetProperty("category").GetString() is string categoryS && categoryS.Equals("Season"))
                {
                    dto.Seasons.Add(entity);
                    success = true;
                }
            }
            if (success) return await stepContext.EndDialogAsync(dto, cancellationToken);
            return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
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
                dto.Season = int.Parse(season);
            }
            return await stepContext.EndDialogAsync(dto, cancellationToken);
        }
    }
}
