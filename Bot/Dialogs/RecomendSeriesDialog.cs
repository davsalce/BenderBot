using Bot.Resorces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using MockSeries;
using MockSeries.Models;

namespace Bot.Dialogs
{
    public class RecomendSeriesDialog : ComponentDialog
    {
        private readonly SeriesClient _seriesClient;

        public RecomendSeriesDialog(SeriesClient seriesClient)
        {
            _seriesClient = seriesClient;
            var waterfallSteps = new WaterfallStep[]
         {
             GetRecomendedSeries,
         };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(RecomendSeriesDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog) + nameof(RecomendSeriesDialog);
        }

        private async Task<DialogTurnResult> GetRecomendedSeries(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ICollection<ExploreShow>? seriesList = await _seriesClient.GetRecomendSeriesAsync();
            List<Attachment> attachments = new List<Attachment>();
            foreach (var series in seriesList)
            {
                HeroCard heroCard = new HeroCard()
                {
                    Title = series.Name,
                    Subtitle = Common.Dialogs_HeroCard_Subtitle(series.Followers, series.Status),
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
            var activity = MessageFactory.Carousel(attachments, RecomendSeries.RecomendSeriesDialog_Carousel);
            await stepContext.Context.SendActivityAsync(activity, cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}