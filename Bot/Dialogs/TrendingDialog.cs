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

        enum Period
        { 
            AllTimes,
            Today,
            ThisWeek,
            ThisMonth,
        }
        public TrendingDialog(ConversationState conversationState, SeriesClient seriesClient)
        {
            _conversationState = conversationState;
            _seriesClient = seriesClient;
            var waterfallSteps = new WaterfallStep[]
         {
                GetPeriodFromCLU,
                AskForPeriodStepAsync,
                AnswerToPeriod,
                GetSeriesbyPeriod
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
            // Crea propieddad para acceder al json que me ha dado el CLU. Este JSON está guardado en el conversationState
            IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");

            // recuperamos el JSON de CLUPredicction
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);


            JsonElement entitiesJson = CLUPrediction.GetProperty("entities");

            JsonElement[] entities = JsonSerializer.Deserialize<JsonElement[]>(entitiesJson);

            Period period = default;

            foreach (JsonElement entity in entities)
            {
                if (entity.GetProperty("category").ToString().Equals("DateTime"))
                {

                    JsonElement resolutionsJson = entity.GetProperty("resolutions");
                    JsonElement[] resolutions = JsonSerializer.Deserialize<JsonElement[]>(resolutionsJson);
                    foreach (JsonElement resolution in resolutions)
                    {
                        if (IsThisMonth(resolution))
                        {
                            period = Period.ThisMonth;
                        }
                        else if (IsThisWeek(resolution))
                        {
                            period = Period.ThisWeek;
                        }
                        else if (IsToday(resolution))
                        {
                            period = Period.Today;
                        }
                    }
                }
            }

            return await stepContext.NextAsync(period, cancellationToken: cancellationToken);
        }

        private static async Task<DialogTurnResult> AskForPeriodStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is not Period || stepContext.Result is Period period && period is not default(Period))
            {

                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("¿Las trending de hoy, de la semana, del mes, o por siempre?"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "Hoy", "Semana", "Mes", "Por siempre" }),
                    }, cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> AnswerToPeriod(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Period period = default;
            if (stepContext.Result is Period period1) period = period1;
            if (stepContext.Result is string periodStr)
            {
                switch (periodStr)
                {
                    case "Hoy":
                        period = Period.Today;
                        break;
                    case "Semana":
                        period = Period.ThisWeek;
                        break;
                    case "Mes":
                        period = Period.ThisMonth;
                        break;
                    case "Por siempre":
                    default:
                        period = Period.AllTimes;
                        break;


                }
            }
            return await stepContext.NextAsync(period, cancellationToken: cancellationToken);
        } 
        
        private Task<DialogTurnResult> GetSeriesbyPeriod(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        private bool IsThisWeek(JsonElement resolution)
        {
            if (resolution.TryGetProperty("begin", out JsonElement beginJson)
               && resolution.TryGetProperty("end", out JsonElement endJson)
               && beginJson.TryGetDateTime(out DateTime begin)
               && endJson.TryGetDateTime(out DateTime end))
            {
                TimeSpan week = new TimeSpan(7, 0, 0, 0);
                if (end >= DateTime.Now.Date && begin <= DateTime.Now.Date
                    && (end - begin).Days == week.Days)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsToday(JsonElement resolution)
        {
            if (resolution.TryGetProperty("value", out JsonElement todayJson)
                && todayJson.TryGetDateTime(out DateTime today)
                && today.Date == DateTime.Now.Date)
            {
                return true;
            }
            return false;
        }

        private static bool IsThisMonth(JsonElement resolution)
        {
            if (resolution.TryGetProperty("begin", out JsonElement beginJson)
                && resolution.TryGetProperty("end", out JsonElement endJson)
                && beginJson.TryGetDateTime(out DateTime begin)
                && endJson.TryGetDateTime(out DateTime end))
            {
                TimeSpan monthLower = new TimeSpan(28, 0, 0, 0);
                TimeSpan monthFreater = new TimeSpan(31, 0, 0, 0);

                if (end - begin >= monthLower && end - begin <= monthFreater)
                {
                    return true;
                }
            }
            return false;
        }


    }
}