using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Text.Json;
using IMiddleware = Microsoft.Bot.Builder.IMiddleware;

namespace Bot.Middleware
{
    public class CLUmiddleware : IMiddleware
    {
        private readonly ConversationAnalysisClient conversationAnalysisClient;
        private readonly ConversationState conversationState;
        public CLUmiddleware(ConversationAnalysisClient conversationAnalysisClient, ConversationState conversationState)
        {
            this.conversationAnalysisClient = conversationAnalysisClient;
            this.conversationState = conversationState;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            BotAssert.ContextNotNull(turnContext);

            if (turnContext.Activity.Type == ActivityTypes.Message)
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
                RequestContent? requestContent = RequestContent.Create(CLUrequestBody);
                Response? response = await conversationAnalysisClient.AnalyzeConversationAsync(requestContent);
                if (response.ContentStream != null)
                {
                    using JsonDocument result = JsonDocument.Parse(response.ContentStream);
                    JsonElement conversationalTaskResult = result.RootElement;
                    JsonElement conversationPrediction = conversationalTaskResult.GetProperty("result").GetProperty("prediction");
                    IStatePropertyAccessor<JsonElement> statePropertyAccessor = conversationState.CreateProperty<JsonElement>("CLUPrediction");
                    await statePropertyAccessor.SetAsync(turnContext, conversationPrediction);
                }
            }
            await next(cancellationToken);    
        }
        
    }
    public record Intent(string category, double confidenceScore);
}
