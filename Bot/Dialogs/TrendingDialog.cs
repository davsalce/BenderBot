using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using MockSeries;
using System.Text.Json;

namespace Bot.Dialogs
{
    public class TrendingDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        private readonly SeriesClient _seriesClient;

        
        public TrendingDialog(ConversationState conversationState, SeriesClient seriesClient)
        {
            _conversationState = conversationState;
            _seriesClient = seriesClient;
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
        public record Resolution(string dateTimeSubKind, DateTime value, DateTime beguin, DateTime end);

        private async Task<DialogTurnResult> GetPeriodFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);


            var resolutions = CLUPrediction.GetProperty("entities").GetProperty("resolutions").Deserialize<List<Resolution>>();
            Resolution? period = resolutions?.FirstOrDefault<Resolution>(resolution =>
                                                                         resolution.value.Equals(DateTime.Date) // hoy
                                                                         
                                                                         );
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
