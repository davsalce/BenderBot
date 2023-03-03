using Bot.Bot.Channels.DirectLine;
using Bot.CognitiveServices.OpenAI;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.Dialogs
{
	public class OpenAiCompleteDialog : Dialog
	{
		private readonly DirectLineClient _directLineClient;
		private readonly OpenAIClient _openAiClient;

		public OpenAiCompleteDialog(DirectLineClient directLineClient ,OpenAIClient openAiClient)
		{
			_directLineClient = directLineClient;
			_openAiClient = openAiClient;
		}

		public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
		{
			ICollection<Activity> activities = await _directLineClient.RetrieveActivities(dc.Context.Activity.Conversation.Id, dc.Context.Activity.Id);
			var openAIAnswer = await _openAiClient.GenerateAnswerWithContext(activities, dc.Context.Activity.From.Id);

			await dc.Context.SendActivityAsync(openAIAnswer, cancellationToken: cancellationToken);
			return await dc.EndDialogAsync();
		}
	}
}
