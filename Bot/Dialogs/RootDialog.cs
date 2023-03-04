using Bot.CognitiveServices.CLU;
using Bot.Dialogs.ChangeLanguage;
using Bot.Dialogs.IntentHandlers;
using Bot.Dialogs.MarkEpisodeAsWatched;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Bot.Dialogs
{
    public class RootDialog : ComponentDialog
    {

        private readonly ConversationState _conversationState;
        private readonly IEnumerable<IIntentHandler> _intentHandlers;

        public RootDialog(ConversationState conversationState,
            IEnumerable<IIntentHandler> intentHandlers,
            CQADialog CQADialog,
            TrendingDialog trendingDialog,
            MarkAsWatchedRootDialog markEpisodeAsWatched,
            PendingEpisodesDialog pendingEpisodesDialog,
            RecommendSeriesDialog recomendSeriesDialog,
            ChangeLanguageDialog changeLanguageDialog,
            OpenAiCompleteDialog openAiCompleteDialog)
        {
            _conversationState = conversationState;
            _intentHandlers = intentHandlers;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GetCLUIntent,
                Redirect
            }));
            AddDialog(CQADialog);
            AddDialog(trendingDialog);
            AddDialog(markEpisodeAsWatched);
            AddDialog(pendingEpisodesDialog);
            AddDialog(recomendSeriesDialog);
            AddDialog(changeLanguageDialog);
            AddDialog(openAiCompleteDialog);

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> GetCLUIntent(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");
            CLUPrediction cLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            return await stepContext.NextAsync(cLUPrediction.TopIntent, cancellationToken);
        }

        private async Task<DialogTurnResult> Redirect(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IIntentHandler intentHandler = default;

            foreach (var handler in _intentHandlers)
            {
                if (await handler.IsValidAsync(stepContext.Context, cancellationToken))
                {
                    intentHandler = handler;
                    break;
                }
            }
            if (intentHandler == null) intentHandler = _intentHandlers.FirstOrDefault(ih => ih.IsDefault());
            return await intentHandler.Handle(stepContext, cancellationToken);
        }
    }
}
