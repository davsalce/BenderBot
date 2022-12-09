using Bot.Dialogs.ChangeLanguage;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

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

        public override async Task Handle(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await _changeLanguageDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        public override async Task<bool> IsValidAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("ChangeLanguage");
        }
    }
}
