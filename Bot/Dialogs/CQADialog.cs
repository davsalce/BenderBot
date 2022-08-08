using Microsoft.Bot.Builder.Dialogs;

namespace Bot.Dialogs
{
    public class CQADialog : ComponentDialog
    {
        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
