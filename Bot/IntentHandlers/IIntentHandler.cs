using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Bot.IntentHandlers
{
    public interface IIntentHandler
    {

        public Task<bool> IsValidAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken);
        public Task Handle(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken);
        public bool IsDefault();
    }
}
