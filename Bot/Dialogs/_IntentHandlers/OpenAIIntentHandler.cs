using Bot.Dialogs.IntentHandlers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Bot.Dialogs._IntentHandlers
{
	public class OpenAIIntentHandler : IntentHandlerBase
	{
		private readonly ConversationState _conversationState;
		private readonly OpenAiCompleteDialog _openAiCompleteDialog;

		public OpenAIIntentHandler(ConversationState conversationState, OpenAiCompleteDialog openAiCompleteDialog) : base(conversationState)
		{
			_conversationState = conversationState;
			_openAiCompleteDialog = openAiCompleteDialog;
		}
		public override async Task Handle(ITurnContext turnContext, CancellationToken cancellationToken)
		{
			await _openAiCompleteDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
		}

		public override async Task<DialogTurnResult> Handle(DialogContext dialogContext, CancellationToken cancellationToken)
		{
			return await dialogContext.BeginDialogAsync(_openAiCompleteDialog.Id, null, cancellationToken);
		}

		public override async Task<bool> IsValidAsync(ITurnContext turnContext, CancellationToken cancellationToken)
		{
			return (await TopIntentAsync(turnContext, cancellationToken)).Equals("None");
		}

		public override bool IsDefault()
		{
			return true;
		}
	}
}