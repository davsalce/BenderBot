using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Prompts;

namespace Bot.Dialogs
{
    public class ChangeLanguageDialog : ComponentDialog
    {
        public ChangeLanguageDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                ChangeLanguage,
                yesorno,
            };
            var culture = new PromptCultureModel()
            {
                InlineOr = " o ",
                InlineOrMore = "",
                Locale = "es-es",
                Separator = ",",
                NoInLanguage = "No",
                YesInLanguage = "Sí",
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(ChangeLanguageDialog), waterfallSteps));
           
            var customDictionary = new Dictionary<string, (Choice, Choice, ChoiceFactoryOptions)>()
            {
                    { culture.Locale,
                                        (
                                        new Choice(culture.YesInLanguage),
                                        new Choice(culture.NoInLanguage),
                                        new ChoiceFactoryOptions(culture.Separator,culture.InlineOr, culture.InlineOrMore, true)
                                        )
                    }
            };
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt) + nameof(ChangeLanguageDialog), customDictionary, null, "es-ES"));
        }
        private async Task<DialogTurnResult> ChangeLanguage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            PromptOptions promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text($"¿Quieres cambiar de idioma?"),
                RetryPrompt = MessageFactory.Text("Responde sí o no."),
                Style = ListStyle.SuggestedAction,
            };
            return await stepContext.PromptAsync(nameof(ConfirmPrompt) + nameof(ChangeLanguageDialog), promptOptions, cancellationToken: cancellationToken);
        }
        
        private async Task<DialogTurnResult> yesorno(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is bool confirmation//SI
               && confirmation)
            {
               
            }
            else//NO
            {
                
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

    }
}
