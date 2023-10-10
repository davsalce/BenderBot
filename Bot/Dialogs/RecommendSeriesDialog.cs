using Bot.Resources;
using Bot.CognitiveServices.OpenAI;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TrackSeries.Core.Client;
using Bot.Bot.Channels.DirectLine;

namespace Bot.Dialogs
{
    public class RecommendSeriesDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        private readonly ITrackSeriesClient _trackseriesClient;
        private readonly OpenAIClient _openAiClient;

        public RecommendSeriesDialog(ConversationState conversationState, ITrackSeriesClient trackseriesClient, OpenAIClient openAiClient, GetTokenDialog getTokenDialog)
        {
            _conversationState = conversationState;
            _trackseriesClient = trackseriesClient;
            _openAiClient = openAiClient;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(RecommendSeriesDialog), new WaterfallStep[]
             {
                GetToken,
                SetToken,
                GetFollowingSeries,
                GenerateRecomendations,                
                SearchRecommendedSeries,
                DisplayRecommendations
             }));
            AddDialog(getTokenDialog);
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog) + nameof(RecommendSeriesDialog);
        }

        private async Task<DialogTurnResult> GetToken(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(GetTokenDialog), cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> SetToken(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IStatePropertyAccessor<TokenResponse> directLineTokenPropertyAccessor = _conversationState.CreateProperty<TokenResponse>("DirectLineToken");
            await directLineTokenPropertyAccessor.SetAsync(stepContext.Context, (TokenResponse)stepContext.Result, cancellationToken: cancellationToken);
            return await stepContext.NextAsync(stepContext.Result, cancellationToken);
        }

        private async Task<DialogTurnResult> GetFollowingSeries(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var tokenResponse = stepContext.Result as TokenResponse;
            ICollection<FollowedShowViewModel>? series = default;
            if (!string.IsNullOrEmpty(tokenResponse?.Token))
            {
                _trackseriesClient.SetCurrentUserToken(tokenResponse.Token);
                series = await _trackseriesClient.GetFollowedShowsAsync(FollowType.Following, cancellationToken);
            }
            return await stepContext.NextAsync(series, cancellationToken);
        }

        private async Task<DialogTurnResult> GenerateRecomendations(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var series = stepContext.Result as ICollection<FollowedShowViewModel>;
            string[]? recommendedSeriesNames = await _openAiClient.GetSeriesRecommendationAsync(series.Select(s => s.Name));
            return await stepContext.NextAsync(recommendedSeriesNames, cancellationToken);
        }

        private async Task<DialogTurnResult> SearchRecommendedSeries(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var recommendedSeriesNames = stepContext.Result as string[];
            ICollection<SearchShowModel> series = default;

            IStatePropertyAccessor<TokenResponse> directLineTokenPropertyAccessor = _conversationState.CreateProperty<TokenResponse>("DirectLineToken");
            TokenResponse? tokenResponse = await directLineTokenPropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            if (!string.IsNullOrEmpty(tokenResponse?.Token))
            {
                _trackseriesClient.SetCurrentUserToken(tokenResponse.Token);
                series = await _trackseriesClient.SearchShowsByNameAsync(recommendedSeriesNames, cancellationToken);
            }
            return await stepContext.NextAsync(series, cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayRecommendations(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var recommendedSeries = stepContext.Result as ICollection<SearchShowModel>;
            IEnumerable<Attachment>? attachments = recommendedSeries?.Select(s => s.ToAttachment());

            var activity = MessageFactory.Carousel(attachments, RecomendSeries.RecomendSeriesDialog_Carousel);
            await stepContext.Context.SendActivityAsync(activity, cancellationToken: cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}