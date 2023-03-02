
using Bot.IntentHandlers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IntentHandlersServicesCollectionExtension
    {
        public static IServiceCollection AddIntentHandlers(this IServiceCollection services)
        {

            builder.Services.AddTransient<IIntentHandler, MarkEpisodeAsWatchedIntentHandler>();
            builder.Services.AddTransient<IIntentHandler, ChangeLanguageIntentHandler>();
            builder.Services.AddTransient<IIntentHandler, PendingEpisodeIntentHandler>();
            builder.Services.AddTransient<IIntentHandler, RecomendSeriesIntentHandler>();
            builder.Services.AddTransient<IIntentHandler, TrendingIntentHander>();
            builder.Services.AddTransient<IIntentHandler, CQAIntentHandler>();

            return services;
        }
    }
}
