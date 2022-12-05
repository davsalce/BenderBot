using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Diagnostics;
using System.Globalization;
using IMiddleware = Microsoft.Bot.Builder.IMiddleware;

namespace Bot.Middleware
{
    public class LanguageMiddleware : IMiddleware
    {
        private readonly ConversationState _conversationState;

        public LanguageMiddleware(ConversationState conversationState)
        {
            _conversationState = conversationState;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            IStatePropertyAccessor<string> converstationStateAccesor = _conversationState.CreateProperty<string>("CurrentCulture");
            string currentCulture = await converstationStateAccesor
                .GetAsync(turnContext, () => turnContext.Activity?.Locale ?? Thread.CurrentThread.CurrentCulture.Name, cancellationToken);

            Debug.WriteLine("converstationState currentCulture: " + currentCulture);
            Debug.WriteLine("turnContext.Activity?.Locale: " + turnContext.Activity?.Locale);
            Debug.WriteLine("turnContext.Activity.Recipient.Id: " + turnContext.Activity?.Recipient.Id);
            Debug.WriteLine("Thread.CurrentThread.CurrentCulture: " + Thread.CurrentThread.CurrentCulture.Name);
            Debug.WriteLine("Thread.CurrentThread.CurrentUICulture: " + Thread.CurrentThread.CurrentUICulture.Name);

            if (turnContext.Activity?.Locale != currentCulture)
                turnContext.Activity.SetLocale(currentCulture);

            if (Thread.CurrentThread.CurrentCulture.Name != currentCulture)
                Thread.CurrentThread.CurrentCulture = new CultureInfo(currentCulture);

            if (Thread.CurrentThread.CurrentUICulture.Name != currentCulture)
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentCulture);


            Debug.WriteLine("converstationState currentCulture: " + currentCulture);
            Debug.WriteLine("turnContext.Activity?.Locale: " + turnContext.Activity?.Locale);
            Debug.WriteLine("turnContext.Activity.Recipient.Id: " + turnContext.Activity?.Recipient.Id);
            Debug.WriteLine("Thread.CurrentThread.CurrentCulture: " + Thread.CurrentThread.CurrentCulture.Name);
            Debug.WriteLine("Thread.CurrentThread.CurrentUICulture: " + Thread.CurrentThread.CurrentUICulture.Name);

            await next(cancellationToken);
        }
    }
}
