using Microsoft.Extensions.Options;

namespace Bot.OpenAI
{
    public class OpenAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAIOptions _options;

        public OpenAIClient(HttpClient httpClient, OpenAIOptions options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        public OpenAIClient(HttpClient httpClient, IOptions<OpenAIOptions> options)
        {
            _httpClient = httpClient;
        }
    }
}
