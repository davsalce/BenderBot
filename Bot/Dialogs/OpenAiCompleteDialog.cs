using Bot.Bot.Channels.DirectLine;
using Bot.CognitiveServices.OpenAI;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.Dialogs
{
	public class OpenAiCompleteDialog : Dialog
	{
		private readonly DirectLineSDKClient _directLineSDKClient;
        private readonly DirectLineClient _directLineClient;
        private readonly OpenAIClient _openAiClient;

		public OpenAiCompleteDialog(DirectLineSDKClient directLineSDKClient, DirectLineClient directLineClient,OpenAIClient openAiClient)
		{
			_directLineSDKClient = directLineSDKClient;
            _directLineClient = directLineClient;
            _openAiClient = openAiClient;
		}

		public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
		{
			var activitiesSDK = await _directLineSDKClient.RetrieveActivities(dc.Context.Activity.Conversation.Id);
			var activities = await _directLineClient.RetrieveActivities(dc.Context.Activity.Conversation.Id, "0");

			var openAIAnswer = await _openAiClient.GenerateAnswerWithContext(activities, dc.Context.Activity.From.Id);

			await dc.Context.SendActivityAsync(openAIAnswer.Replace("Bender:", string.Empty), cancellationToken: cancellationToken);
			return await dc.EndDialogAsync();
		}
	}
}
