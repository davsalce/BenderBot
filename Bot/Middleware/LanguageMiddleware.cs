using Microsoft.Bot.Builder;
using IMiddleware = Microsoft.Bot.Builder.IMiddleware;

namespace Bot.Middleware
{
    public class LanguageMiddleware : IMiddleware
    {
        public LanguageMiddleware()
        {
                
        }
        public Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
