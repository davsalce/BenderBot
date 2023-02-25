using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.IntentHandlers
{
    public class HelpIntentHandler : IntentHandlerBase
    {
        private readonly ConversationState _conversationState;
        private HelpDialog _helperDialog;

        public HelpIntentHandler(ConversationState conversationState, HelpDialog helperDialog) : base(conversationState)
        {
            _conversationState = conversationState;
            _helperDialog = helperDialog;
        }

        public override async Task Handle(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await _helperDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        public override async Task<bool> IsValidAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("Help");
        }
    }
}
