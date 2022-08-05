using Azure;
using Azure.AI.Language.Conversations;
using Azure.AI.Language.QuestionAnswering;
using Azure.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Text.Json;

namespace Bot.Bots
{
    public class BenderBot : ActivityHandler
    {
        private readonly QuestionAnsweringClient questionAnsweringClient;
        private readonly QuestionAnsweringProject questionAnsweringProject;
        private readonly ConversationAnalysisClient conversationAnalysisClient;
        public BenderBot(QuestionAnsweringClient questionAnsweringClient, QuestionAnsweringProject questionAnsweringProject, ConversationAnalysisClient conversationAnalysisClient)
        {
            this.questionAnsweringClient = questionAnsweringClient;
            this.questionAnsweringProject = questionAnsweringProject;
            this.conversationAnalysisClient = conversationAnalysisClient;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Hola", cancellationToken: cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string textMessage = turnContext.Activity.Text;
            var CLUrequestBody = new
            {
                analysisInput = new
                {
                    conversationItem = new
                    {
                        text = textMessage,
                        id = turnContext.Activity.Id,
                        participantId = turnContext.Activity.From.Id,
                    }
                },
                parameters = new
                {
                    projectName = "ts-bot-CLU",
                    deploymentName = "TSbotCLUdeployment",

                    // Use Utf16CodeUnit for strings in .NET.
                    stringIndexType = "Utf16CodeUnit",
                },
                kind = "Conversation",
            };
            Response? response = await conversationAnalysisClient.AnalyzeConversationAsync(RequestContent.Create(CLUrequestBody));
            if (response.ContentStream != null)
            {
                using JsonDocument result = JsonDocument.Parse(response.ContentStream);
                JsonElement conversationalTaskResult = result.RootElement;
                JsonElement conversationPrediction = conversationalTaskResult.GetProperty("result").GetProperty("prediction");



                var intents = conversationPrediction.GetProperty("intents").Deserialize<List<Intent>>();
                if (intents.Take(3).All(intent =>
                    intent.category.Equals("RecomendSeries") ||
                    intent.category.Equals("PendingEpisodes") ||
                    intent.category.Equals("TrendingSeries"))) 
                {
                    await turnContext.SendActivityAsync(string.Join(", ", intents), cancellationToken: cancellationToken);
                }
            }

            AnswersOptions answersOptions = new AnswersOptions()
            {
                ConfidenceThreshold = 0.9,
                IncludeUnstructuredSources = true,
                ShortAnswerOptions = new ShortAnswerOptions()
                {
                    ConfidenceThreshold = 0.1
                }
            };

            Response<AnswersResult> customQuestionAnsweringResult = await this.questionAnsweringClient.GetAnswersAsync(textMessage, questionAnsweringProject, answersOptions);
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
        }
    }
    public record Intent(string category, double confidenceScore);
}
