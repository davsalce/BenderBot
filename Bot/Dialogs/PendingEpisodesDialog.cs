using Bot.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using MockSeries;
using MockSeries.Models;

namespace Bot.Dialogs
{
    public class PendingEpisodesDialog : ComponentDialog
    {
        private readonly SeriesClient _seriesClient;

        public PendingEpisodesDialog(SeriesClient seriesClient)
        {
            _seriesClient = seriesClient;
            var waterfallSteps = new WaterfallStep[]
         {
                GetPendingSeries,
         };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(PendingEpisodesDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog) + nameof(PendingEpisodesDialog);
        }
        private async Task<DialogTurnResult> GetPendingSeries(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ICollection<ExploreShow>? seriesList = await _seriesClient.GetPendingEpisodesAsync(stepContext.Context.Activity.From.Id, cancellationToken: cancellationToken);
            List<Attachment> attachments = new List<Attachment>();
            foreach (var series in seriesList)
            {
                HeroCard heroCard = new HeroCard()
                {
                    Title = series.Name,
                    Subtitle = series.Subtitle,
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

            var activity = MessageFactory.Carousel(attachments, PendingEpisodes.PendingEpisodesDialog_Carousel);
            await stepContext.Context.SendActivityAsync(activity, cancellationToken: cancellationToken); 
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
