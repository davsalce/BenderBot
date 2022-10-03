using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Text.Json;
using IMiddleware = Microsoft.Bot.Builder.IMiddleware;

namespace Bot.Middleware
{
    public class CLUMiddleware : IMiddleware
    {
        private readonly ConversationAnalysisClient conversationAnalysisClient;
        private readonly ConversationState _conversationState;
        public CLUMiddleware(ConversationAnalysisClient conversationAnalysisClient, ConversationState conversationState)
        {
            this.conversationAnalysisClient = conversationAnalysisClient;
            this._conversationState = conversationState;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            BotAssert.ContextNotNull(turnContext);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                IStatePropertyAccessor<bool> CLUFlagStatePropertyAccessor = _conversationState.CreateProperty<bool>("CLUFlag");
                bool doNotOverrideCLU = await CLUFlagStatePropertyAccessor.GetAsync(turnContext, cancellationToken: cancellationToken);

                if (!doNotOverrideCLU)
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
                            deploymentName = "TSbotCLUdeployamentV5",

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
                        JsonElement conversationPrediction = conversationalTaskResult.GetProperty("result").GetProperty("prediction").Clone();
                        IStatePropertyAccessor<JsonElement> statePropertyAccessor = _conversationState.CreateProperty<JsonElement>("CLUPrediction");
                        await statePropertyAccessor.SetAsync(turnContext, conversationPrediction);
                    }
                }
            }
            await next(cancellationToken);
        }

    }
    public record Intent(string category, double confidenceScore);
}