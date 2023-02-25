using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Bot.Dialogs
{
    public class HelpDialog : ComponentDialog
    {
        public HelpDialog()
        {

            var waterfallSteps = new WaterfallStep[]
            {
               ShowHelp,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(HelpDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt) + nameof(HelpDialog)));

            InitialDialogId = nameof(WaterfallDialog) + nameof(HelpDialog);
        }

        private async Task<DialogTurnResult> ShowHelp(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Random random = new Random();
            var randomIndex = random.Next(4);

            List<(string button, string desc)> helpOptions = new List<(string, string)>()
            {
                new(Resources.Help.Help_ButtonChangeLanguage, Resources.Help.Help_ChangeLanguage),
                new(Resources.Help.Help_ButtonMarkEpisodeAsWatched, Resources.Help.Help_MarkEpisodeAsWatched),
                new(Resources.Help.Help_ButtonTrending, Resources.Help.Help_Trending),
                new(Resources.Help.Help_ButtonPendingEpisodes, Resources.Help.Help_PendingEpisodes),
                new(Resources.Help.Help_ButtonRecomendSeries, Resources.Help.Help_RecomendSeries)
            };
            string prompt;
            if (stepContext.Options is string promptDialogParameter) 
            {
                prompt = promptDialogParameter;
            }
            else
            {
                prompt = Resources.Help.Help_PromptSchema(helpOptions[randomIndex].button, helpOptions[randomIndex].desc);
            }
            PromptOptions options = CreateOptions(helpOptions, prompt);
            await stepContext.PromptAsync(nameof(ChoicePrompt) + nameof(HelpDialog), options, cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
        private static PromptOptions CreateOptions(List<(string button, string desc)> helpOptions, string prompt)
        {
            PromptOptions options = new PromptOptions()
            {
                Choices = new List<Choice>(),
                Prompt = MessageFactory.Text(prompt),
                Style = ListStyle.SuggestedAction
            };

            foreach (var option in helpOptions)
            {
                options.Choices.Add(new Choice()
                {
                    Value = option.button,
                    Action = new CardAction()
                    {
                        Type = ActionTypes.ImBack,
                        Value = option.button,
                        DisplayText = option.button,
                        Title = option.button,
                        Text = option.button
                    }
                });
            }
            options.Choices.Add(new Choice()
            {
                Value = Resources.Help.Help_ButtonAll,
                Action = new CardAction()
                {
                    Type = ActionTypes.ImBack,
                    Value = Resources.Help.Help_ButtonAll,
                    DisplayText = Resources.Help.Help_ButtonAll,
                    Title = Resources.Help.Help_ButtonAll,
                    Text = Resources.Help.Help_ButtonAll
                }
            });
            return options;
        }
    }
}