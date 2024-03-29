﻿using Bot.Dialogs;
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

        public override async Task Handle(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await _pendingEpisode.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        public override async Task<bool> IsValidAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("PendingEpisodes");
        }
    }
}
