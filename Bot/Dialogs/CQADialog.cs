using Azure;
using Azure.AI.Language.QuestionAnswering;
using Microsoft.Bot.Builder.Dialogs;

namespace Bot.Dialogs
{
    public class CQADialog : Dialog
    {
        private readonly QuestionAnsweringClient _questionAnsweringClient;
        private readonly QuestionAnsweringProject _questionAnsweringProject;

        public CQADialog(QuestionAnsweringClient _questionAnsweringClient, QuestionAnsweringProject _questionAnsweringProject)
        {
            this._questionAnsweringClient = _questionAnsweringClient;
            this._questionAnsweringProject = _questionAnsweringProject;
        }
        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
        {
            AnswersOptions answersOptions = new AnswersOptions()
            {
                ConfidenceThreshold = 0.9,
                IncludeUnstructuredSources = true,
                ShortAnswerOptions = new ShortAnswerOptions()
                {
                    ConfidenceThreshold = 0.1
                }
            };

            Response<AnswersResult> customQuestionAnsweringResult = await _questionAnsweringClient.GetAnswersAsync(dc.Context.Activity.Text, _questionAnsweringProject, answersOptions);
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
                await dc.Context.SendActivityAsync(text, cancellationToken: cancellationToken);
            }
            return await dc.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}