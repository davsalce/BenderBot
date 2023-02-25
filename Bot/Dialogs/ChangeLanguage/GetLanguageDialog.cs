using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

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
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt) + nameof(GetLanguageDialog)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetLanguageDialog);
        }

        private async Task<DialogTurnResult> AskForLanguage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(Resources.ChangeLanguage.ChangeLanguage_AskForLanguage),
                RetryPrompt = MessageFactory.Text(Resources.ChangeLanguage.ChangeLanguage_AskForLanguageRetryPrompt),
                Choices = new List<Choice>() {
                
                    new Choice()
                    {
                        Synonyms = new List<string>(){"ingles", "español", "espanol", "espanyol", "english", "English", "Ingles", "inglés", "Inglés"},
                        Value = Resources.ChangeLanguage.GetLanguage_ChoiceSpanish,
                        Action = new CardAction()
                        {
                            Type = ActionTypes.ImBack,
                            Value = Resources.ChangeLanguage.GetLanguage_ChoiceSpanish,
                            DisplayText = Resources.ChangeLanguage.GetLanguage_ChoiceSpanish,
                            Title = Resources.ChangeLanguage.GetLanguage_ChoiceSpanish,
                            Text = Resources.ChangeLanguage.GetLanguage_ChoiceSpanish
                        }
                    },
                    new Choice()
                    {
                        Synonyms = new List<string>(){"ingles", "español", "espanol", "espanyol", "english", "English", "Ingles", "inglés", "Inglés"},
                        Value = Resources.ChangeLanguage.GetLanguage_ChoiceEnglish,
                         Action = new CardAction()
                         {
                            Type = ActionTypes.ImBack,
                            Value =  Resources.ChangeLanguage.GetLanguage_ChoiceEnglish,
                            DisplayText =  Resources.ChangeLanguage.GetLanguage_ChoiceEnglish,
                            Title =  Resources.ChangeLanguage.GetLanguage_ChoiceEnglish,
                            Text =  Resources.ChangeLanguage.GetLanguage_ChoiceEnglish
                         }
                    },
                    new Choice()
                    {
                        Synonyms = new List<string>(){"cancelar", "Cancela", "cancela", "No", "no", "salir", "exit", "Cancel", "cancel", "Salir"},
                        Value = Resources.ChangeLanguage.GetLanguage_Cancel,
                        Action = new CardAction()
                         {
                            Type = ActionTypes.ImBack,
                            Value =  Resources.ChangeLanguage.GetLanguage_Cancel,
                            DisplayText =  Resources.ChangeLanguage.GetLanguage_Cancel,
                            Title =  Resources.ChangeLanguage.GetLanguage_Cancel,
                            Text =  Resources.ChangeLanguage.GetLanguage_Cancel
                         }
                    }
                },
            };
            return await stepContext.PromptAsync(nameof(ChoicePrompt) + nameof(GetLanguageDialog), promptOptions, cancellationToken);
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
