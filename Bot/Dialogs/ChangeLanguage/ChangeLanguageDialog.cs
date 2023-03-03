using Bot.CognitiveServices.CLU;
using Bot.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Globalization;
using System.Reflection;
using static Bot.CognitiveServices.CLU.CLUPrediction;

namespace Bot.Dialogs.ChangeLanguage
{
    public class ChangeLanguageDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        private readonly GetLanguageDialog _getLanguageDialog;

        public ChangeLanguageDialog(ConversationState conversationState, GetLanguageDialog getLanguageDialog)
        {
            _conversationState = conversationState;
            _getLanguageDialog = getLanguageDialog;
            var waterfallSteps = new WaterfallStep[]
            {
                GetLanguageFromCLU,
                GetLanguageFromUser,
                ConfirmChangeLanguage,
                SetLanguage,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(ChangeLanguageDialog), waterfallSteps));
            AddDialog(_getLanguageDialog);
            Dictionary<string, (Choice, Choice, ChoiceFactoryOptions)> customDictionary = DialogHelper.InitializeConfirmChoiceDictionary();
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt) + nameof(ChangeLanguageDialog), customDictionary, null, Common.Spanish));
        }
   
        private async Task<DialogTurnResult> GetLanguageFromCLU(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string? language = await GetLanguageFromCLUAux(stepContext, cancellationToken);
            return await stepContext.NextAsync(language, cancellationToken: cancellationToken);
        }

        private async Task<string> GetLanguageFromCLUAux(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");
            CLUPrediction cLUPrediction = await statePropertyAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);
            string? language = null;
            foreach (Entity entity in cLUPrediction.Entities.Where(e=>e.ExtraInformation is not null))
            {
                language = entity.GetLanguageFromEntities();
            }

            return language;
        }

        private async Task<DialogTurnResult> GetLanguageFromUser(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is not string
                || stepContext.Result == null)
            {
                return await stepContext.BeginDialogAsync(_getLanguageDialog.Id, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmChangeLanguage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is string language
                && !string.IsNullOrEmpty(language)
                && !stepContext.Context.Activity.Locale.Contains(language))
            {
                language = await GetLanguageFromCLUAux(stepContext, cancellationToken);
                stepContext.Values.Add("CurrentCulture", language);

                if (stepContext.Dialogs.Find(nameof(ConfirmPrompt) + nameof(ChangeLanguageDialog)) is ConfirmPrompt d)
                {
                    d.DefaultLocale = stepContext.Context.Activity.Locale;
                }

                string friendlyLanguageName = GetFriendlyLanguageName(language);

                PromptOptions promptOptions = new PromptOptions()
                {
                    Prompt = MessageFactory.Text(Resources.ChangeLanguage.ChangeLanguage_Question(friendlyLanguageName)),
                    RetryPrompt = MessageFactory.Text(Common.CheckConfirmation_RetryPrompt),
                    Style = ListStyle.SuggestedAction,
                };
                return await stepContext.PromptAsync(nameof(ConfirmPrompt) + nameof(ChangeLanguageDialog), promptOptions, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Result, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// aux method to transform the key language of CLU to a friendly name (ex. es --> español).
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        private static string GetFriendlyLanguageName(string language)
        {
            System.Resources.ResourceManager resourceManager
            = new System.Resources.ResourceManager("Bot.Resources.ChangeLanguage", typeof(Resources.ChangeLanguage).GetTypeInfo().Assembly);

            CultureInfo cLUCulture = new CultureInfo(language);

            string friendlyLanguageName = resourceManager.GetString("CurrentLanguage", cLUCulture);
            return friendlyLanguageName;
        }

        private async Task<DialogTurnResult> SetLanguage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is bool confirmation
               && confirmation)
            {
                string? language = stepContext.Values["CurrentCulture"] as string;
                IStatePropertyAccessor<string> converstationStateAccesor = _conversationState.CreateProperty<string>("CurrentCulture");
                await converstationStateAccesor.SetAsync(stepContext.Context, language, cancellationToken);
                await stepContext.Context.SendActivityAsync(Resources.ChangeLanguage.ChangeLanguage_ConfirmChange, cancellationToken: cancellationToken);
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
