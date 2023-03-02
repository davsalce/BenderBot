using Microsoft.Extensions.Options;
using System.Net.Http;

namespace Bot.OpenAI
{
    public static class OpenAIServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenAIClient(this IServiceCollection services, Action<OpenAIOptions> configureOptions)
        {
            // Add options for OpenAI using configuration action 
            services.AddOptions<OpenAIOptions>().Configure(configureOptions);
            // Add HTTP client for OpenAI using typed client pattern 
            services.AddHttpClient<OpenAIClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<OpenAIOptions>>().Value;
                client.BaseAddress = new Uri(options.Endpoint);

                client.DefaultRequestHeaders.Add("api-key", options.Credential);
                client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            });
            return services;
        }

        public static IServiceCollection AddOpenAIClient(this IServiceCollection services, IConfiguration configuration)
        {
            // Add options for OpenAI using configuration section 
            services.AddOptions<OpenAIOptions>().Bind(configuration.GetSection("OpenAI")).ValidateDataAnnotations();
            // Add HTTP client for OpenAI using typed client pattern 
            services.AddHttpClient<OpenAIClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<OpenAIOptions>>().Value;
                client.BaseAddress = new Uri(options.Endpoint);

                client.DefaultRequestHeaders.Add("api-key", options.Credential);
            });
            return services;
        }
    }
}
