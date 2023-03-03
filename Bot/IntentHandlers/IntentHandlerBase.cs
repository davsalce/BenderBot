using Bot.CLU;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.IntentHandlers
{
    public abstract class IntentHandlerBase : IIntentHandler
    {
        private readonly ConversationState _conversationState;

        public IntentHandlerBase(ConversationState conversationState)
        {
            _conversationState = conversationState;
        }
        public abstract Task Handle(ITurnContext turnContext, CancellationToken cancellationToken);
        public abstract Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken);
        public abstract Task<bool> IsValidAsync(ITurnContext turnContext, CancellationToken cancellationToken);

        public async Task<string> TopIntentAsync(ITurnContext turnContext, CancellationToken cancellationToken) 
        {
            IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");
            CLUPrediction cLUPrediction = await statePropertyAccessor.GetAsync(turnContext, cancellationToken: cancellationToken);
            return cLUPrediction.TopIntent;
        }

        public virtual bool IsDefault()
        {
            return false;
        }
    }
}
