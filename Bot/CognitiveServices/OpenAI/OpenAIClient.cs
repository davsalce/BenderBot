using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Bot.CognitiveServices.OpenAI
{
    public class OpenAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAIOptions _options;
        public OpenAIClient(HttpClient httpClient, IOptions<OpenAIOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        /// Method that calls Azure OpenAI completions endpoint using options.SeriesRecommendationOptions and returning an OpenAIResponse
        public async Task<string[]?> GetSeriesRecommendationAsync(IEnumerable<string> enumerable)
        {
            var prompt = new
            {
                Prompt = GetPrompt(enumerable),
                _options.SeriesRecommendations.Max_tokens,
                _options.SeriesRecommendations.Temperature,
                _options.SeriesRecommendations.Frequency_penalty,
                _options.SeriesRecommendations.Presence_penalty,
                _options.SeriesRecommendations.Top_p,
                _options.SeriesRecommendations.Best_of,
                _options.SeriesRecommendations.Stop
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "openai/deployments/text-davinci-003/completions?api-version=2022-12-01")
            {
                Content = new StringContent(JsonSerializer.Serialize(prompt, new JsonSerializerOptions(JsonSerializerDefaults.Web)), Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request);

            var content = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
            return content?.Choices?.FirstOrDefault()?.Text.Split(',');
        }

        /// Get prompt given a list of seriesId
        private string GetPrompt(IEnumerable<string> seriesNames)
        {
            StringBuilder prompt = new StringBuilder();

            prompt.Append("Given watched series: ");
            foreach (string seriesName in seriesNames)
            {
                prompt.Append($"{seriesName}");
                prompt.Append($", ");
            }
            prompt.AppendLine(" make 5 series recommendation.");
            prompt.Append("Generate a comma separated list with the name of each recommendation.");
            return prompt.ToString();
        }
    }
}