using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.IntentHandlers
{
    public class TrendingIntentHander : IntentHandlerBase
    {
        private readonly ConversationState _conversationState;
        private TrendingDialog _trendingDialog;

        public TrendingIntentHander(ConversationState conversationState, TrendingDialog trendingDialog) : base(conversationState)
        {
            _conversationState = conversationState;
            _trendingDialog = trendingDialog;
        }

        public override async Task Handle(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await _trendingDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        public override async Task<bool> IsValidAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("TrendingSeries"); 
        }
    }
}
