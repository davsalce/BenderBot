﻿using Azure;
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
        private readonly ConversationState conversationState;

        public BenderBot(QuestionAnsweringClient questionAnsweringClient, QuestionAnsweringProject questionAnsweringProject, ConversationState conversationState)
        {
            this.questionAnsweringClient = questionAnsweringClient;
            this.questionAnsweringProject = questionAnsweringProject;
            this.conversationState = conversationState;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Hola", cancellationToken: cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            IStatePropertyAccessor<JsonElement> statePropertyAccessor = conversationState.CreateProperty<JsonElement>("CLUPrediction");
            JsonElement CLUPrediction = await statePropertyAccessor.GetAsync(turnContext, cancellationToken :cancellationToken);
            string? topIntent = CLUPrediction.GetProperty("topIntent").GetString();
            if (string.IsNullOrEmpty(topIntent) || string.Equals(topIntent,"None")) {
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
            }
        }
    }
}
