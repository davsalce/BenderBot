using Bot.Dialogs.ChangeLanguage;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;

namespace Bot.IntentHandlers
{
    public class ChangeLanguageIntentHandler : IntentHandlerBase
    {
        private readonly ConversationState _conversationState;
        private ChangeLanguageDialog _changeLanguageDialog;
        
        public ChangeLanguageIntentHandler(ConversationState conversationState, ChangeLanguageDialog changeLanguageDialog) : base(conversationState)
        {
            _conversationState = conversationState;
            _changeLanguageDialog = changeLanguageDialog;
        }

        public override async Task Handle(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await _changeLanguageDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        public override async Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            return await dialogContext.BeginDialogAsync(_changeLanguageDialog.Id, null, cancellationToken);
        }

        public override async Task<bool> IsValidAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("ChangeLanguage");
        }
    }
}
