using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using MockSeries;

namespace Bot.Dialogs.MarkEpisodeAsWatched
{
    public class GetSeriesNameDialog : ComponentDialog
    {
        private readonly SeriesClient _seriesClient;

        public GetSeriesNameDialog(SeriesClient seriesClient) : base(nameof(GetSeriesNameDialog))
        {
            _seriesClient = seriesClient;
            var waterfallSteps = new WaterfallStep[]
            {
                AskForSeriesName,
                CheckSeriesNameFromTS,
                ConfirmationSeriesName
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog) + nameof(GetSeriesNameDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt) + nameof(GetSeriesNameDialog)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt) + nameof(CheckSeriesNameFromTS)));
            InitialDialogId = nameof(WaterfallDialog) + nameof(GetSeriesNameDialog);

        }

        private async Task<DialogTurnResult> AskForSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Options is MarkEpisodeAsWatchDTO dto
            && string.IsNullOrEmpty(dto.SeriesName))
            {
                return await stepContext.PromptAsync(nameof(TextPrompt) + nameof(GetSeriesNameDialog), new PromptOptions
                { Prompt = MessageFactory.Text(Resources.MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_EpisodeDialog_AskForSeriesName) }, cancellationToken);
            }
            return await stepContext.NextAsync(stepContext.Options, cancellationToken: cancellationToken);
        }
        private async Task<DialogTurnResult> CheckSeriesNameFromTS(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MarkEpisodeAsWatchDTO? dto = default;
            if (stepContext.Result is MarkEpisodeAsWatchDTO dtoResult)
            {
                dto = dtoResult;
            }
            else if (stepContext.Result is string seriesName
                && stepContext.Options is MarkEpisodeAsWatchDTO dtoOptions)
            {
                dto = dtoOptions;
                dto.SeriesName = seriesName;
            }

            IList<string> seriesNames = _seriesClient.GetSeriesNames(stepContext.Context.Activity.From.Id, dto.SeriesName);
            if (!seriesNames.Any(s => s.Equals(dto.SeriesName)))
            {
                List < Choice >  choices = new List<Choice>();
                foreach (var serieName in seriesNames)
                {
                    Choice choice= new Choice() 
                    {
                        Action = new CardAction
                        {
                            Title = serieName,
                            Value = serieName,
                            Type = ActionTypes.ImBack
                        },
                        Value = serieName
                    };
                    choices.Add(choice);    
                }

                PromptOptions promptOptions = new PromptOptions()
                {
                    Prompt = MessageFactory.Text(Resources.MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_EpisodeDialog_Text),
                    RetryPrompt = MessageFactory.Text(Resources.MarkEpisodeAsWhatched.MarkEpisodeAsWatchedDialog_EpisodeDialog_RetryPrompt),
                    Style = ListStyle.SuggestedAction,
                    Choices = choices
                };
                return await stepContext.PromptAsync(nameof(ChoicePrompt) + nameof(CheckSeriesNameFromTS), promptOptions, cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(dto, cancellationToken: cancellationToken);
        }
        private async Task<DialogTurnResult> ConfirmationSeriesName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MarkEpisodeAsWatchDTO dto = stepContext.Options as MarkEpisodeAsWatchDTO;
            if (stepContext.Result is FoundChoice seriesName
               && seriesName is not null)
            {
                dto.SeriesName = seriesName.Value;
            }
            else if (stepContext.Result is string seriesNameString
                && !string.IsNullOrEmpty(seriesNameString)) 
            {
                dto.SeriesName = seriesNameString;
            }
            return await stepContext.EndDialogAsync(dto, cancellationToken);
        }
    }
}
