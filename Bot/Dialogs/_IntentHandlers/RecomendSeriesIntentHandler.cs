using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.Dialogs.IntentHandlers
{
    public class RecomendSeriesIntentHandler : IntentHandlerBase
    {
        private readonly ConversationState _conversationState;
        RecommendSeriesDialog _recomendSeriesDialog;
        public RecomendSeriesIntentHandler(ConversationState conversationState, RecommendSeriesDialog recomendSeriesDialog) : base(conversationState)
        {
            _conversationState = conversationState;
            _recomendSeriesDialog = recomendSeriesDialog;
        }

        public override async Task Handle(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await _recomendSeriesDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        public override async Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            return await dialogContext.BeginDialogAsync(_recomendSeriesDialog.Id, null, cancellationToken);
        }

        public override async Task<bool> IsValidAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("RecomendSeries");
        }
    }
}
