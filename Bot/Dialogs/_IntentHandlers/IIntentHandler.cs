using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.Dialogs.IntentHandlers
{
    public interface IIntentHandler
    {
        Task<bool> IsValidAsync(ITurnContext turnContext, CancellationToken cancellationToken);
        Task Handle(ITurnContext turnContext, CancellationToken cancellationToken);
        Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken);
        bool IsDefault();
    }
}
