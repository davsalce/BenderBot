﻿
using Bot.IntentHandlers;
using Intelequia.Core.TurismoTenerife.Bot.IntentHandlers;

namespace Microsoft.Extensions.DependencyInjection
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
            services.AddTransient<IIntentHandler, CQAIntentHandler>();
            services.AddTransient<IIntentHandler, IntentHandlerCancel>();

            return services;
        }
    }
}
