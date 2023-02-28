using Bot.Middleware;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;

namespace Bot
{
    public class BenderAdapter : CloudAdapter
    {
        public BenderAdapter(BotFrameworkAuthentication botFrameworkAuthentication,
            ILogger<CloudAdapter> logger,
            CLUMiddleware CLUMiddleware,
            LanguageMiddleware languageMiddleware) : base(botFrameworkAuthentication, logger)
        {
            Use(languageMiddleware);
            Use(CLUMiddleware);

            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                if (exception is not ArgumentOutOfRangeException ñapa)
                    // Send a catch-all apology to the user.
                    await turnContext.SendActivityAsync("Something failed. I'm so embarrassed. I wish everybody else was dead.");
            };
        }
    }
}
