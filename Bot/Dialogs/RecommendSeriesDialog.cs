using Bot.DirectLine;
using Bot.OpenAI;
using Bot.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TrackSeries.Core.Client;

namespace Bot.Dialogs
{
    public class RecommendSeriesDialog : ComponentDialog
    {
        private readonly ITrackSeriesClient _trackseriesClient;
        private readonly OpenAIClient _openAiClient;

        public RecommendSeriesDialog(ITrackSeriesClient trackseriesClient, OpenAIClient openAiClient)
        {
            _trackseriesClient = trackseriesClient;
            _openAiClient = openAiClient;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(RecommendSeriesDialog), new WaterfallStep[]
             {
                LogIn,
                 GetFollowingSeries,
                 GenerateRecomendations,
                 SearchRecommendedSeries,
                 DisplayRecommendations
             }));

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog) + nameof(RecommendSeriesDialog);
        }

        private async Task<DialogTurnResult> LogIn(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Sign in is needed to check your pending episodes!"), cancellationToken);
            return await stepContext.BeginDialogAsync(nameof(GetTokenDialog), cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GetFollowingSeries(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var tokenResponse = stepContext.Result as TokenResponse;
            ICollection<FollowedShowViewModel>? series = default;
            if (!string.IsNullOrEmpty(tokenResponse?.Token))
            {
                series = await _trackseriesClient.GetFollowedShowsAsync(FollowType.Following, cancellationToken);                
            }
            return await stepContext.NextAsync(series, cancellationToken);
        }

        private async Task<DialogTurnResult> GenerateRecomendations(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var series = stepContext.Result as ICollection<FollowedShowViewModel>;
            string[]? recommendedSeriesNames = await _openAiClient.GetSeriesRecommendationAsync(series.Select(s=>s.Name));
            return await stepContext.NextAsync(recommendedSeriesNames, cancellationToken);
        }

        private async Task<DialogTurnResult> SearchRecommendedSeries(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var recommendedSeriesNames = stepContext.Result as string[];
            var series = new List<SearchShowViewModel>();
            return await stepContext.NextAsync(series, cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayRecommendations(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var recommendedSeries = stepContext.Result as ICollection<SearchShowViewModel>;
            IEnumerable<Attachment>? attachments = recommendedSeries?.Select(s => s.ToAttachment());

            var activity = MessageFactory.Carousel(attachments, RecomendSeries.RecomendSeriesDialog_Carousel);
            await stepContext.Context.SendActivityAsync(activity, cancellationToken: cancellationToken);
            
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}