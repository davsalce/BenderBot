using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.IntentHandlers
{
    public class RecomendSeriesIntentHandler : IntentHandlerBase
    {
        private readonly ConversationState _conversationState;
        RecomendSeriesDialog _recomendSeriesDialog;
        public RecomendSeriesIntentHandler(ConversationState conversationState , RecomendSeriesDialog recomendSeriesDialog) : base(conversationState)
        {
            _conversationState = conversationState;
            _recomendSeriesDialog = recomendSeriesDialog;
        }

        public override async Task Handle(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await _recomendSeriesDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        public override async Task<bool> IsValidAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("RecomendSeries");
        }
    }
}
