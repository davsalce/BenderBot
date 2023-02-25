﻿// <auto-generated />

using System;
using System.Reflection;
using System.Resources;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Bot.Resources
{
    internal static class Help
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Bot.Resources.Help", typeof(Help).GetTypeInfo().Assembly);

        /// <summary>
        ///     Tell me more
        /// </summary>
        public static string Help_ButtonAll
            => GetString("Help_ButtonAll");

        /// <summary>
        ///     Change language
        /// </summary>
        public static string Help_ButtonChangeLanguage
            => GetString("Help_ButtonChangeLanguage");

        /// <summary>
        ///     Mark episode
        /// </summary>
        public static string Help_ButtonMarkEpisodeAsWatched
            => GetString("Help_ButtonMarkEpisodeAsWatched");

        /// <summary>
        ///     Pending episodes
        /// </summary>
        public static string Help_ButtonPendingEpisodes
            => GetString("Help_ButtonPendingEpisodes");

        /// <summary>
        ///     Recomend series
        /// </summary>
        public static string Help_ButtonRecomendSeries
            => GetString("Help_ButtonRecomendSeries");

        /// <summary>
        ///     Trending series
        /// </summary>
        public static string Help_ButtonTrending
            => GetString("Help_ButtonTrending");

        /// <summary>
        ///     I'll swap language inmediately between Spanish and English. 
        /// </summary>
        public static string Help_ChangeLanguage
            => GetString("Help_ChangeLanguage");

        /// <summary>
        ///     and I'm inform about the series name, the season and chapter number you have just watched i'll mark it in TrackSeries.
        ///     Also i can mark the whole season you already watched.
        /// </summary>
        public static string Help_MarkEpisodeAsWatched
            => GetString("Help_MarkEpisodeAsWatched");

        /// <summary>
        ///     I'll display your pending episodes.
        /// </summary>
        public static string Help_PendingEpisodes
            => GetString("Help_PendingEpisodes");

        /// <summary>
        ///     If [{button}] button is selected{desc}.
        /// </summary>
        public static string Help_PromptSchema(object button, object desc)
            => string.Format(
                GetString("Help_PromptSchema", nameof(button), nameof(desc)),
                button, desc);

        /// <summary>
        ///     I'll recomend you some series.
        /// </summary>
        public static string Help_RecomendSeries
            => GetString("Help_RecomendSeries");

        /// <summary>
        ///     I'll show you the trending series of the day/week/month.
        /// </summary>
        public static string Help_Trending
            => GetString("Help_Trending");

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);
            for (var i = 0; i < formatterNames.Length; i++)
            {
                value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
            }

            return value;
        }
    }
}

namespace Bot.Resources.Internal
{
    internal static class Help
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Bot.Resources.Help", typeof(Help).GetTypeInfo().Assembly);
    }
}
