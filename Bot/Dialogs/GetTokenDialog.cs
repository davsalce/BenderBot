﻿using Bot.Bot.Channels.DirectLine;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Options;

namespace Bot.Dialogs
{
    public class GetTokenDialog : ComponentDialog
    {
        public GetTokenDialog(IOptions<OAuthOptions> oAuthOptions) : base(nameof(GetTokenDialog))
        {
            AddDialog(new WaterfallDialog(nameof(GetTokenDialog) + nameof(WaterfallDialog), new WaterfallStep[]
            {
                CheckChannelData,
                LogIn
            }));
            AddDialog(new OAuthPrompt(nameof(OAuthPrompt), new OAuthPromptSettings()
            {
                ConnectionName = oAuthOptions.Value.ConnectionName,
                Title = "Sign In"
            }));
        }

        private async Task<DialogTurnResult> CheckChannelData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ChannelData channelData = stepContext.Context.Activity.GetChannelData<ChannelData>();
            if (channelData?.TokenResponse is not null)
            {
                return new DialogTurnResult(DialogTurnStatus.Complete, channelData.TokenResponse);
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> LogIn(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Sign in is needed to check your pending episodes!"), cancellationToken);
            return await stepContext.BeginDialogAsync(nameof(OAuthPrompt), cancellationToken: cancellationToken);
        }
    }
}
