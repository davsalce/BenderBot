using Bot.Dialogs;
using Bot.Dialogs.ChangeLanguage;
using Bot.Dialogs.MarkEpisodeAsWatched;
using Bot.DirectLine;
using Bot.Middleware;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DialogServiceCollectionExtension
    {
        public static IServiceCollection AddDialogs(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<CQADialog>();

            services.AddSingleton<TrendingDialog>();
            services.AddTransient<MarkAsWatchedRootDialog>();
            services.AddSingleton<PendingEpisodesDialog>();
            services.AddSingleton<RecommendSeriesDialog>();
            services.AddSingleton<MarkSeasonAsWatchedDialog>();
            services.AddSingleton<MarkEpisodeAsWatchedDialog>();
            services.AddSingleton<GetSeriesNameDialog>();
            services.AddSingleton<GetSeasonDialog>();
            services.AddSingleton<GetEpisodeDialog>();
            services.AddTransient<ChangeLanguageDialog>();
            services.AddSingleton<GetLanguageDialog>();
            services.AddSingleton<DialogHelper>();

            services.AddSingleton<GetTokenDialog>();
            services.Configure<OAuthOptions>(configuration.GetSection(nameof(OAuthOptions)));

            return services;
        }
    }
}
