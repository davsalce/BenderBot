using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Bot.Bots
{
    public class BenderBot : ActivityHandler
    {
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, 
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Hola", cancellationToken: cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            string textMessage = turnContext.Activity.Text;
            await turnContext.SendActivityAsync(textMessage, cancellationToken: cancellationToken);
        }

    }
}
