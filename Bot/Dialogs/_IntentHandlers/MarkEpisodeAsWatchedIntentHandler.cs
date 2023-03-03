﻿using Bot.Dialogs;
using Bot.Dialogs.MarkEpisodeAsWatched;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.Dialogs.IntentHandlers
{
    public class MarkEpisodeAsWatchedIntentHandler : IntentHandlerBase
    {
        private readonly ConversationState _conversationState;
        private MarkAsWatchedRootDialog _markEpisodeAsWatched;

        public MarkEpisodeAsWatchedIntentHandler(ConversationState conversationState, MarkAsWatchedRootDialog markEpisodeAsWatched) : base(conversationState)
        {
            _conversationState = conversationState;
            _markEpisodeAsWatched = markEpisodeAsWatched;
        }

        public override async Task Handle(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await _markEpisodeAsWatched.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }
        public override async Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            return await dialogContext.BeginDialogAsync(_markEpisodeAsWatched.Id, null, cancellationToken);
        }

        public override async Task<bool> IsValidAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return (await TopIntentAsync(turnContext, cancellationToken)).Equals("MarkEpisodeAsWatched");
        }
    }
}