using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using System.Text.Json;
using Bot.Dialogs.MarkEpisodeAsWatched;

namespace Bot.Bots
{
    public class BenderBot : ActivityHandler
    {
        private readonly ConversationState _conversationState;
        private readonly CQADialog _cQADialog;
        private readonly TrendingDialog _trendingDialog;
        private readonly MarkEpisodeAsWatchedDialog _markEpisodeAsWatched;
        private readonly PendingEpisodesDialog _pendingEpisodesDialog;
        private readonly RecomendSeriesDialog _recomendSeriesDialog;
        public BenderBot(ConversationState conversationState, CQADialog CQADialog, TrendingDialog trendingDialog, MarkEpisodeAsWatchedDialog markEpisodeAsWatched, PendingEpisodesDialog pendingEpisodesDialog, RecomendSeriesDialog recomendSeriesDialog)
        {
            this._conversationState = conversationState;
            _cQADialog = CQADialog;
            _trendingDialog = trendingDialog;
            _markEpisodeAsWatched = markEpisodeAsWatched;
            _pendingEpisodesDialog = pendingEpisodesDialog;
            _recomendSeriesDialog = recomendSeriesDialog;
        }
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);

        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Hola", cancellationToken: cancellationToken);

        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            DialogSet dialogSet = new DialogSet(_conversationState.CreateProperty<DialogState>("DialogState"));
            
            dialogSet.Add(_recomendSeriesDialog); 
            dialogSet.Add(_pendingEpisodesDialog);
            dialogSet.Add(_markEpisodeAsWatched);
            dialogSet.Add(_trendingDialog);
            dialogSet.Add(_cQADialog);

            DialogContext dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken);
            DialogTurnResult results = await dialogContext.ContinueDialogAsync(cancellationToken);
            

            IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(turnContext, cancellationToken: cancellationToken);

             
            IStatePropertyAccessor<DialogState> dialogStatePropertyAccesor = _conversationState.CreateProperty<DialogState>("DialogState");
            string? topIntent = CLUPrediction.GetProperty("topIntent").GetString();

            if (results.Status == DialogTurnStatus.Empty)
            {
                switch (topIntent)
                {
                    case "MarkEpisodeAsWatched":
                        await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                        //Ejecuta el dialogo de Mark
                        await _markEpisodeAsWatched.RunAsync(turnContext, dialogStatePropertyAccesor, cancellationToken);
                        break;
                    case "PendingEpisodes":
                        await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                        await _pendingEpisodesDialog.RunAsync(turnContext, dialogStatePropertyAccesor, cancellationToken);
                        break;
                    case "TrendingSeries":
                        // Envía Actividad de tipo texto, con el texto "TrendingSeries"
                        await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                        // Ejecuta el diálogo de Trending
                        await _trendingDialog.RunAsync(turnContext, dialogStatePropertyAccesor, cancellationToken);
                        break;
                    case "RecomendSeries":
                        await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                        await _recomendSeriesDialog.RunAsync(turnContext, dialogStatePropertyAccesor, cancellationToken);
                        break;
                    case "None":
                    default:
                        await _cQADialog.RunAsync(turnContext, dialogStatePropertyAccesor, cancellationToken);
                        break;
                }
            }
        }
    }
}