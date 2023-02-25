using Microsoft.Bot.Builder.Dialogs;

namespace Bot.Dialogs
{
    public class MoreDialog : ComponentDialog
    {
        public MoreDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
               ShowMoreInformation,
            };


        }

        private async Task<DialogTurnResult> ShowMoreInformation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
