using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;
using Bot.CognitiveServices.CLU;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Text.Json;
using IMiddleware = Microsoft.Bot.Builder.IMiddleware;

namespace Bot.Bot.Middleware
{
    public class CLUMiddleware : IMiddleware
    {
        private readonly ConversationAnalysisClient conversationAnalysisClient;
        private readonly ConversationState _conversationState;
        public CLUMiddleware(ConversationAnalysisClient conversationAnalysisClient, ConversationState conversationState)
        {
            this.conversationAnalysisClient = conversationAnalysisClient;
            _conversationState = conversationState;
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
                    var CLUrequestBody = new
                    {
                        analysisInput = new
                        {
                            conversationItem = new
                            {
                                text = turnContext.Activity.Text,
                                id = turnContext.Activity.Id,
                                participantId = turnContext.Activity.From.Id,
                                language = turnContext.Activity.Locale
                            }
                        },
                        parameters = new
                        {
                            projectName = "ts-bot-CLU",
                            deploymentName = "TSbotCLUdeploymentV10",

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


                        IStatePropertyAccessor<CLUPrediction> statePropertyAccessor = _conversationState.CreateProperty<CLUPrediction>("CLUPrediction");

                        CLUPrediction? cluPrediction = conversationPrediction.Deserialize<CLUPrediction>(
                            new JsonSerializerOptions()
                            {
                                Converters =
                                { new ValueCustomJsonConverter() }
                            });
                        if (cluPrediction is not null)
                            await statePropertyAccessor.SetAsync(turnContext, cluPrediction);
                    }
                }
            }
            await next(cancellationToken);


        }
    }



    public record Intent(string category, double confidenceScore);
}