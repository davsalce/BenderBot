using Bot.Dialogs._IntentHandlers;

namespace Bot.Dialogs.IntentHandlers
{
    public static class IntentHandlersServicesCollectionExtension
    {
        public static IServiceCollection AddIntentHandlers(this IServiceCollection services)
        {

            services.AddTransient<IIntentHandler, MarkEpisodeAsWatchedIntentHandler>();
            services.AddTransient<IIntentHandler, ChangeLanguageIntentHandler>();
            services.AddTransient<IIntentHandler, PendingEpisodeIntentHandler>();
            services.AddTransient<IIntentHandler, RecomendSeriesIntentHandler>();
            services.AddTransient<IIntentHandler, TrendingIntentHander>();
			services.AddTransient<IIntentHandler, IntentHandlerCancel>();
			services.AddTransient<IIntentHandler, OpenAIIntentHandler>();
            services.AddTransient<IIntentHandler, CQAIntentHandler>();

            return services;
        }
    }
}
