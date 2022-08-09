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

        public BenderBot(ConversationState conversationState, CQADialog CQADialog)
        {
            this._conversationState = conversationState;
            _cQADialog = CQADialog;
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
                    await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                    break;
                case "RecomendSeries":
                    await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                    break;
                case "None":
                default:
                    IStatePropertyAccessor<DialogState> dialogStatePropertyAccesor = _conversationState.CreateProperty<DialogState>("DialogState");
                    await _cQADialog.RunAsync( turnContext, dialogStatePropertyAccesor,  cancellationToken);
                    break;
            }
        }
    }
}