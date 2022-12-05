using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Bot.Dialogs.ChangeLanguage
{
    public class GetLanguageDialog : ComponentDialog
    {
        public GetLanguageDialog() : base(nameof(GetLanguageDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                AskForLanguage,
                ConfirmationLanguage
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(GetLanguageDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt) + nameof(GetLanguageDialog)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetLanguageDialog);
        }

        private async Task<DialogTurnResult> AskForLanguage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(Resources.ChangeLanguage.ChangeLanguage_AskForLanguage),
                Choices = new List<Choice>() { new Choice() { Value = Resources.ChangeLanguage.GetLanguage_ChoiceSpanish } , new Choice() { Value = Resources.ChangeLanguage.GetLanguage_ChoiceEnglish } },
            };
            return await stepContext.PromptAsync(nameof(TextPrompt) + nameof(GetLanguageDialog), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmationLanguage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string? language = null;
            if (stepContext.Result is string languageUser
               && !string.IsNullOrEmpty(languageUser))
            {
                language = languageUser;
            }
            return await stepContext.EndDialogAsync(language, cancellationToken);
        }
    }
}
