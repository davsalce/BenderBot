using Azure;
using Azure.AI.Language.QuestionAnswering;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Bot.Bots
{
    public class BenderBot : ActivityHandler
    {
        private readonly QuestionAnsweringClient questionAnsweringClient;
        private readonly QuestionAnsweringProject questionAnsweringProject;
        public BenderBot(QuestionAnsweringClient questionAnsweringClient, QuestionAnsweringProject questionAnsweringProject)
        {
            this.questionAnsweringClient = questionAnsweringClient;
            this.questionAnsweringProject = questionAnsweringProject;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Hola", cancellationToken: cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string textMessage = turnContext.Activity.Text;

           
            AnswersOptions answersOptions = new AnswersOptions()
            {
                ConfidenceThreshold = 0.8,
                IncludeUnstructuredSources = true
            };

            Response<AnswersResult> customQuestionAnsweringResult = await this.questionAnsweringClient.GetAnswersAsync(textMessage, questionAnsweringProject);
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
}
