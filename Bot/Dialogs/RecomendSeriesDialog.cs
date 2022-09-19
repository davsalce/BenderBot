using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using MockSeries;

namespace Bot.Dialogs
{
    public class RecomendSeriesDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        private readonly SeriesClient _seriesClient;
        public RecomendSeriesDialog(ConversationState _conversationState, SeriesClient _seriesClient, ConversationState conversationState, SeriesClient seriesClient)
        {
            this._conversationState = conversationState;
            this._seriesClient = seriesClient;

            var waterfallSteps = new WaterfallStep[]
            {
                GetRecomendedSeries
            };
        }

        private Task<DialogTurnResult> GetRecomendedSeries(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
