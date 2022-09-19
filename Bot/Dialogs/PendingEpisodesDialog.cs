using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using MockSeries;

namespace Bot.Dialogs
{
    public class PendingEpisodesDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        private readonly SeriesClient _seriesClient;
        public PendingEpisodesDialog(ConversationState _conversationState, SeriesClient _seriesClient, ConversationState conversationState, SeriesClient seriesClient)
        {
            this._conversationState = conversationState;
            this._seriesClient = seriesClient;

            var waterfallSteps = new WaterfallStep[]
            {
                GetPendingSeries
                //ir a algun sitio a mirar las series que sigue y no a terminado
                //devolverselas
            };
        }
        private Task<DialogTurnResult> GetPendingSeries(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
