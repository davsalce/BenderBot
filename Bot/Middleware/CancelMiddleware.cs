using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using IMiddleware = Microsoft.Bot.Builder.IMiddleware;

namespace Bot.Middleware
{
    public class CancelMiddleware : IMiddleware
    {
        private readonly ConversationState _conversationState;
        public CancelMiddleware(ConversationState conversationState)
        {
            _conversationState = conversationState;
        }
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        { 
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                string? text = turnContext.Activity.Text.ToLowerInvariant();

                if (text == "cancelar" || text == "cancel")
                {
                    await turnContext.SendActivityAsync("Diálogo cancelado.");
                }
                else
                {
                    await next(cancellationToken);
                }
            }
            else
            {
                await next(cancellationToken);
            }
        }
    }
}