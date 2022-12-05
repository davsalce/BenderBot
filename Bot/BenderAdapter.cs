using Bot.Middleware;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace Bot
{
    public class BenderAdapter : CloudAdapter
    {
        public BenderAdapter(CLUMiddleware CLUMiddleware, LanguageMiddleware languageMiddleware) : base()
        {
            Use(languageMiddleware);
            Use(CLUMiddleware);        
        }

    }
}
