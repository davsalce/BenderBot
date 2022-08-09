using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Text.Json;

namespace Bot.Dialogs
{
    public class TrendingDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;

        public record Entity(string category, string text);
        public TrendingDialog(ConversationState conversationState)
        {
            _conversationState = conversationState;
            var waterfallSteps = new WaterfallStep[]
         {
                GetPeriodFromCLU,
                AskForPeriodStepAsync,
         };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            //AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), AgePromptValidatorAsync));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            //AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt), PicturePromptValidatorAsync));

            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> GetPeriodFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);


            var entities = CLUPrediction.GetProperty("entities").Deserialize<List<Entity>>();
            Entity? period = entities?.FirstOrDefault<Entity>(entity =>
                                                                        entity.category.Equals("DateTime")
                                                                        && (entity.text.Equals("hoy")
                                                                            || entity.text.Equals("semana")
                                                                            || entity.text.Equals("mes")
                                                                            || entity.text.Equals("por siempre")));
            return await stepContext.NextAsync(period, cancellationToken: cancellationToken);
        }

        private static async Task<DialogTurnResult> AskForPeriodStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is Entity entity)
            {
                return await stepContext.NextAsync(entity, cancellationToken: cancellationToken);
            }
            else

                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("¿Las trending de hoy, de la semana, del mes, o por siempre?"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "Hoy", "Semana", "Mes", "Por siempre" }),
                    }, cancellationToken);
        }
    }
}
