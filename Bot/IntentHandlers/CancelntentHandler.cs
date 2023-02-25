using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.IntentHandlers
{
    public class CancelntentHandler : IntentHandlerBase
    {
        public CancelntentHandler(ConversationState conversationState) : base(conversationState)
        {      
            
        }

        public override async Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            return await dialogContext.CancelAllDialogsAsync();
        }

        public override Task Handle(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async override Task<bool> IsValidAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("Cancel");
        }
    }
}
