﻿using Bot.CLU;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using MockSeries;
using MockSeries.Models;
using Entity = Bot.CLU.CLUPrediction.Entity;

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
                AnswerToPeriod,
                GetSeriesbyPeriod
         };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(TrendingDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            //AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), AgePromptValidatorAsync));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            //AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt), PicturePromptValidatorAsync));

            InitialDialogId = nameof(WaterfallDialog) + nameof(TrendingDialog);
        }


        public record Resolution(string dateTimeSubKind, DateTime value, DateTime beguin, DateTime end);

        private async Task<DialogTurnResult> GetPeriodFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Crea propieddad para acceder al json que me ha dado el CLU. Este obejto está guardado en el conversationState
            IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");

            // recuperamos el objeto de CLUPredicction
            CLUPrediction cLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            MockSeries.Models.TrendingPeriod period = default;

            foreach (Entity entity in cLUPrediction.Entities)
            {
                if (entity.Category.Equals("DateTime"))
                {
                    foreach (CLUPrediction.Resolution resolution in entity.Resolutions)
                    {
                        if (IsThisMonth(resolution))
                        {
                            period = TrendingPeriod.LastMonth;
                            break;
                        }
                        else if (IsThisWeek(resolution))
                        {
                            period = TrendingPeriod.LastWeek;
                            break;
                        }
                        else if (IsToday(resolution))
                        {
                            period = TrendingPeriod.Today;
                            break;
                        }
                    }
                }
            }

            return await stepContext.NextAsync(period, cancellationToken: cancellationToken);
        }

        private static async Task<DialogTurnResult> AskForPeriodStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is not TrendingPeriod
                || stepContext.Result is TrendingPeriod period
                && period is default(TrendingPeriod))
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
            TrendingPeriod period = default;
            if (stepContext.Result is TrendingPeriod period1) period = period1;
            if (stepContext.Result is string periodStr)
            {
                switch (periodStr)
                {
                    case "Hoy":
                        period = TrendingPeriod.Today;
                        break;
                    case "Semana":
                        period = TrendingPeriod.LastWeek;
                        break;
                    case "Mes":
                        period = TrendingPeriod.LastMonth;
                        break;
                    case "Por siempre":
                    default:
                        period = TrendingPeriod.AllTimes;
                        break;
                }
            }
            return await stepContext.NextAsync(period, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetSeriesbyPeriod(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is TrendingPeriod period)
            {
                ICollection<ExploreShow>? seriesList = await _seriesClient.GetTrendingShowsAsync(period, cancellationToken: cancellationToken);
                List<Attachment> attachments = new List<Attachment>();
                foreach (var series in seriesList)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = series.Name,
                        Subtitle = $"Followers: {series.Followers} Status: {series.Status}",
                        Text = series.Overview,
                        Images = new List<CardImage>()
                        {
                            new CardImage()
                            {
                                Url = series.Image
                            }
                        },
                    };
                    attachments.Add(heroCard.ToAttachment());
                }

                var activity = MessageFactory.Carousel(attachments, "Aqui tienes.");
                await stepContext.Context.SendActivityAsync(activity, cancellationToken: cancellationToken); 
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }



        private bool IsThisWeek(CLUPrediction.Resolution resolution)
        {

            if (resolution.End != null && resolution.Begin != null)
            {
                TimeSpan week = new TimeSpan(7, 0, 0, 0);
                if (resolution.End >= DateTime.Now.Date && resolution.Begin <= DateTime.Now.Date
                    && (resolution.End - resolution.Begin)?.Days == week.Days)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsToday(CLUPrediction.Resolution resolution)
        {
            if (resolution.Value is DateTime today
                && today.Date == DateTime.Now.Date)
            {
                return true;
            }
            return false;
        }

        private static bool IsThisMonth(CLUPrediction.Resolution resolution)
        {
            if (resolution.End != null && resolution.Begin != null)
            {
                TimeSpan monthLower = new TimeSpan(28, 0, 0, 0);
                TimeSpan monthFreater = new TimeSpan(31, 0, 0, 0);

                if ((resolution.End - resolution.Begin >= monthLower && (resolution.End - resolution.Begin <= monthFreater)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}