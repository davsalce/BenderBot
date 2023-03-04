using Bot.Bot.Channels.DirectLine;
using Microsoft.Bot.Schema;
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

        public async Task<string> GenerateAnswerWithContext(DirectLineActivitiesResult activities, string userId)
        {
            var prompt = new
            {
                Prompt = PrepareConversation(activities, userId),
                Max_tokens = _options.CompletionParameters.Max_tokens,
                Temperature = _options.CompletionParameters.Temperature,
                Frequency_penalty = _options.CompletionParameters.Frequency_penalty,
                Presence_penalty = _options.CompletionParameters.Presence_penalty,
                Top_p = _options.CompletionParameters.Top_p,
                Best_of = _options.CompletionParameters.Best_of,
                Stop = "User"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "openai/deployments/text-davinci-003/completions?api-version=2022-12-01")
            {
                Content = new StringContent(JsonSerializer.Serialize(prompt, new JsonSerializerOptions(JsonSerializerDefaults.Web)), Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request);

            var content = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
            return content?.Choices?.FirstOrDefault()?.Text;
        }

        /// Method that calls Azure OpenAI completions endpoint using options.SeriesRecommendationOptions and returning an OpenAIResponse
        public async Task<string[]?> GetSeriesRecommendationAsync(IEnumerable<string> seriesNames)
        {
            var prompt = new
            {
                Prompt = GetPrompt(seriesNames),
                Max_tokens = _options.CompletionParameters.Max_tokens,
                Temperature = _options.CompletionParameters.Temperature,
                Frequency_penalty = _options.CompletionParameters.Frequency_penalty,
                Presence_penalty = _options.CompletionParameters.Presence_penalty,
                Top_p = _options.CompletionParameters.Top_p,
                Best_of = _options.CompletionParameters.Best_of,
                Stop = "user"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "openai/deployments/text-davinci-003/completions?api-version=2022-12-01")
            {
                Content = new StringContent(JsonSerializer.Serialize(prompt, new JsonSerializerOptions(JsonSerializerDefaults.Web)), Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request);

            var content = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
            return content?.Choices?.FirstOrDefault()?.Text.Split(',');
        }


        internal string PrepareConversation(DirectLineActivitiesResult activities, string userId)
        {
            StringBuilder prompt = new StringBuilder();
            prompt.Append("This is a conversation between Futurama's robot: 'Bender' and a user: ");

            foreach (var activity in activities.activities)
            {
                if (activity.from.id == "TSBenderBot-v2")
                {
                    prompt.Append("Bender: ");
                    prompt.AppendLine(activity.text);
                }
                else
                {
                    prompt.Append("User: ");
                    prompt.AppendLine(activity.text);
                }
            }
            return prompt.ToString();
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