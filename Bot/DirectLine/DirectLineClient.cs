using System.Text;
using System.Text.Json;

namespace Bot.DirectLine
{
    public class DirectLineClient
    {
        private readonly HttpClient _httpClient;

        public DirectLineClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DirectLineTokenModel> GenerateTokenAsync()
        {
            var postBody = new
            {
                User = new { Id = $"dl_{Guid.NewGuid()}" },
                TrustedOrigins = new string[] { "http://localhost", "", "https://bender.trackseries.tv" }
            };

            var jsonPostBody = JsonSerializer.Serialize(postBody);

            var stringContent = new StringContent(jsonPostBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://directline.botframework.com/v3/directline/tokens/generate", stringContent);

            if (response.IsSuccessStatusCode)
            {
                DirectLineTokenModel token = await response.Content.ReadFromJsonAsync<DirectLineTokenModel>();
                return token;
            }
            return null;
        }

        public async Task<DirectLineTokenModel> Reconnect(string conversationId, int watermark)
        {
            DirectLineTokenModel directLineToken = await _httpClient.GetFromJsonAsync<DirectLineTokenModel>(
                $"https://directline.botframework.com/v3/directline/conversations/{conversationId}?watermark={watermark}");

            return directLineToken;
        }
    }
}
