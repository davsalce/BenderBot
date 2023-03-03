using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.IntentHandlers
{
    public class PendingEpisodeIntentHandler : IntentHandlerBase
    {
        private readonly ConversationState _conversationState;
        private PendingEpisodesDialog _pendingEpisode;
        public PendingEpisodeIntentHandler(ConversationState conversationState, PendingEpisodesDialog pendingEpisode) : base(conversationState)
        {
            _conversationState = conversationState;
            _pendingEpisode = pendingEpisode;
        }

        public override async Task Handle(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await _pendingEpisode.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        public override async Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            return await dialogContext.BeginDialogAsync(_pendingEpisode.Id, null, cancellationToken);
        }

        public override async Task<bool> IsValidAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("PendingEpisodes");
        }
    }
}
