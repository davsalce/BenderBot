using Bot.IntentHandlers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Intelequia.Core.TurismoTenerife.Bot.IntentHandlers
{
    public class IntentHandlerCancel : IntentHandlerBase
    {
        public IntentHandlerCancel(ConversationState conversationState) : base(conversationState)
        {
        }
        
        public override async Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            return await dialogContext.CancelAllDialogsAsync();
        }

        public override Task Handle(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> IsValidAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("Cancel");
        }
    }
}
