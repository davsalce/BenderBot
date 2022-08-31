using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using System.Text.Json;

namespace Bot.Bots
{
    public class BenderBot : ActivityHandler
    {
        private readonly ConversationState _conversationState;
        private readonly CQADialog _cQADialog;
        private readonly TrendingDialog _trendingDialog;

        public BenderBot(ConversationState conversationState, CQADialog CQADialog, TrendingDialog trendingDialog)
        {
            this._conversationState = conversationState;
            _cQADialog = CQADialog;
            _trendingDialog = trendingDialog;
        }
        public override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            return base.OnTurnAsync(turnContext, cancellationToken);

        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Hola", cancellationToken: cancellationToken);

        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(turnContext, cancellationToken: cancellationToken);


            IStatePropertyAccessor<DialogState> dialogStatePropertyAccesor = _conversationState.CreateProperty<DialogState>("DialogState");

            string? topIntent = CLUPrediction.GetProperty("topIntent").GetString();
            switch (topIntent)
            {
                case "MarkEpisodeAsWatched":
                    await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                    break;
                case "PendingEpisodes":
                    await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                    break;
                case "TrendingSeries":
                    // Envía Actividad de tipo texto, con el texto "TrendingSeries"
                    await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                    // Ejecuta el diálogo de Trending
                    await _trendingDialog.RunAsync(turnContext, dialogStatePropertyAccesor, cancellationToken);
                    break;
                case "RecomendSeries":
                    await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                    break;
                case "None":
                default:
                    await _cQADialog.RunAsync( turnContext, dialogStatePropertyAccesor,  cancellationToken);
                    break;
            }
        }
    }
}