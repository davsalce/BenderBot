using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.IntentHandlers
{
    public class MoreIntentHandler : IntentHandlerBase
    {
        private readonly ConversationState _conversationState;
        private MoreDialog _moreDialog;

        public MoreIntentHandler(ConversationState conversationState, MoreDialog moreDialog) : base(conversationState)
        {
            _conversationState = conversationState;
            _moreDialog = moreDialog;
        }
        public override async Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            return await dialogContext.BeginDialogAsync(_moreDialog.Id, cancellationToken: cancellationToken);
        }

        public async override Task Handle(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await _moreDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        public async override Task<bool> IsValidAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("More");
        }
    }
}
