using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.IntentHandlers
{
    public class CQAIntentHandler : IntentHandlerBase
    {
        private readonly ConversationState _conversationState;
        private CQADialog _cQADialog;
        public CQAIntentHandler(ConversationState conversationState, CQADialog cQADialog) : base(conversationState)
        {
            _conversationState = conversationState;
            _cQADialog = cQADialog;
        }

        public override async Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            return await dialogContext.BeginDialogAsync(_cQADialog.Id, cancellationToken: cancellationToken);
        }

        public override async Task Handle(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await _cQADialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }
        public override async Task<bool> IsValidAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("None"); 
        }
        public override bool IsDefault()
        {
            return true;
        }
    }
}
