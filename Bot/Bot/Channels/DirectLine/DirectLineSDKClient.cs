using Microsoft.AspNetCore.DataProtection;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Extensions.Options;

namespace Bot.Bot.Channels.DirectLine
{
    public class DirectLineSDKClient
    {
        private readonly Microsoft.Bot.Connector.DirectLine.DirectLineClient _client;

        public DirectLineSDKClient(IOptions<DirectLineOptions> options)
        {
            _client = new Microsoft.Bot.Connector.DirectLine.DirectLineClient(secretOrToken: options.Value.SecretKey);
        }

        public async Task<Conversation> GenerateTokenAsync()
        {
            return await _client.Tokens.GenerateTokenForNewConversationAsync();
            //return tokenResponse.Token;
        }

        public async Task<Conversation> Reconnect(string conversationId, string watermark = null)
        {
            return await _client.Conversations.ReconnectToConversationAsync(conversationId, watermark);
        }

        public async Task<ActivitySet> RetrieveActivities(string conversationId, string watermark = null)
        {
            return await _client.Conversations.GetActivitiesAsync(conversationId, watermark);
        }
    }
}