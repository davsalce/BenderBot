﻿// <auto-generated />

using System;
using System.Reflection;
using System.Resources;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Bot.Resources
{
    internal static class Common
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Bot.Resources.Common", typeof(Common).GetTypeInfo().Assembly);

        /// <summary>
        ///     Answer yes or no.
        /// </summary>
        public static string CheckConfirmation_RetryPrompt
            => GetString("CheckConfirmation_RetryPrompt");

        /// <summary>
        ///     Followers: {followers} Status: {status}
        /// </summary>
        public static string Dialogs_HeroCard_Subtitle(object followers, object status)
            => string.Format(
                GetString("Dialogs_HeroCard_Subtitle", nameof(followers), nameof(status)),
                followers, status);

        /// <summary>
        ///     en-US
        /// </summary>
        public static string English
            => GetString("English");

        /// <summary>
        ///     es-ES
        /// </summary>
        public static string Spanish
            => GetString("Spanish");

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
    internal static class Common
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Bot.Resources.Common", typeof(Common).GetTypeInfo().Assembly);
    }
}
