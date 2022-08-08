using Azure;
using Azure.AI.Language.QuestionAnswering;
using Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Text.Json;

namespace Bot.Bots
{
    public class BenderBot : ActivityHandler
    {
        private readonly QuestionAnsweringClient questionAnsweringClient;
        private readonly QuestionAnsweringProject questionAnsweringProject;
        private readonly ConversationState conversationState;
        private readonly CQADialog _CQADialog;

        public BenderBot(QuestionAnsweringClient questionAnsweringClient, QuestionAnsweringProject questionAnsweringProject,
            ConversationState conversationState, CQADialog CQADialog)
        {
            this.questionAnsweringClient = questionAnsweringClient;
            this.questionAnsweringProject = questionAnsweringProject;
            this.conversationState = conversationState;
            _CQADialog = CQADialog;
        }
        public override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            return base.OnTurnAsync(turnContext, cancellationToken);

        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Hola", cancellationToken: cancellationToken);

        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            IStatePropertyAccessor<JsonElement> statePropertyAccessor = conversationState.CreateProperty<JsonElement>("CLUPrediction");
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(turnContext, cancellationToken: cancellationToken);

            string? topIntent = CLUPrediction.GetProperty("topIntent").GetString();
            switch (topIntent)
            {
                case "MarkEpisodeAsWatched":
                    await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                    break;
                case "PendingEpisodes":
                    await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                    break;
                case "TrendingSeries":
                    await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                    break;
                case "RecomendSeries":
                    await turnContext.SendActivityAsync(topIntent, cancellationToken: cancellationToken);
                    break;
                case "None":
                default:

                    AnswersOptions answersOptions = new AnswersOptions()
                    {
                        ConfidenceThreshold = 0.9,
                        IncludeUnstructuredSources = true,
                        ShortAnswerOptions = new ShortAnswerOptions()
                        {
                            ConfidenceThreshold = 0.1
                        }
                    };

                    Response<AnswersResult> customQuestionAnsweringResult = await this.questionAnsweringClient.GetAnswersAsync(turnContext.Activity.Text, questionAnsweringProject, answersOptions);
                    AnswersResult? answersResult = customQuestionAnsweringResult.Value;
                    List<KnowledgeBaseAnswer>? knowledgeBaseAnswers = answersResult.Answers as List<KnowledgeBaseAnswer>;

                    if (knowledgeBaseAnswers != null && knowledgeBaseAnswers.Any())
                    {
                        KnowledgeBaseAnswer? knowledgeBaseAnswer = knowledgeBaseAnswers.FirstOrDefault();
                        string? text = knowledgeBaseAnswer?.ShortAnswer?.Text;
                        if (text == null)
                        {
                            text = knowledgeBaseAnswer?.Answer;
                        }
                        await turnContext.SendActivityAsync(text, cancellationToken: cancellationToken);
                    }
                    break;
            }
        }
    }
}