using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using Bot.Dialogs.MarkEpisodeAsWatched;
using Bot.Dialogs.ChangeLanguage;
using Bot.IntentHandlers;

namespace Bot.Bots
{
    public class BenderBot : ActivityHandler
    {
        private readonly ConversationState _conversationState;
        private readonly IEnumerable<IIntentHandler> _intentHandlers;
        private readonly CQADialog _cQADialog;
        private readonly TrendingDialog _trendingDialog;
        private readonly MarkAsWatchedRootDialog _markEpisodeAsWatched;
        private readonly PendingEpisodesDialog _pendingEpisodesDialog;
        private readonly RecomendSeriesDialog _recomendSeriesDialog;
        private readonly ChangeLanguageDialog _changeLanguageDialog;
        private readonly HelpDialog _helperDialog; 
        private readonly MoreDialog _moreDialog;
        public BenderBot(ConversationState conversationState, IEnumerable<IIntentHandler> intentHandlers, CQADialog CQADialog, TrendingDialog trendingDialog, MarkAsWatchedRootDialog markEpisodeAsWatched, PendingEpisodesDialog pendingEpisodesDialog, RecomendSeriesDialog recomendSeriesDialog, ChangeLanguageDialog changeLanguageDialog, HelpDialog helperDialog, MoreDialog moreDialog)
        {
            this._conversationState = conversationState;
            _intentHandlers = intentHandlers;
            _cQADialog = CQADialog;
            _trendingDialog = trendingDialog;
            _markEpisodeAsWatched = markEpisodeAsWatched;
            _pendingEpisodesDialog = pendingEpisodesDialog;
            _recomendSeriesDialog = recomendSeriesDialog;
            _changeLanguageDialog = changeLanguageDialog;
            _helperDialog = helperDialog;
            _moreDialog = moreDialog;

        }
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await _helperDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            DialogSet dialogSet = new DialogSet(_conversationState.CreateProperty<DialogState>("DialogState"));
            dialogSet.Add(_recomendSeriesDialog);
            dialogSet.Add(_pendingEpisodesDialog);
            dialogSet.Add(_markEpisodeAsWatched);
            dialogSet.Add(_trendingDialog);
            dialogSet.Add(_cQADialog);
            dialogSet.Add(_changeLanguageDialog);
            dialogSet.Add(_helperDialog); 
            dialogSet.Add(_moreDialog);

            DialogContext dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken);
            DialogTurnResult results = await dialogContext.ContinueDialogAsync(cancellationToken);

            if (results.Status == DialogTurnStatus.Empty)
            {

                IIntentHandler intentHandler = default;

                foreach (var handler in _intentHandlers)
                {
                    if (await handler.IsValidAsync(turnContext, cancellationToken))
                    {
                        intentHandler = handler;                     
                        break;
                    }
                }
                if (intentHandler == null) intentHandler = _intentHandlers.FirstOrDefault(ih => ih.IsDefault());
                await intentHandler.Handle(turnContext, cancellationToken);
            }
        }
    }
}