﻿// <auto-generated />

using System;
using System.Reflection;
using System.Resources;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Bot.Resources
{
    internal static class ChangeLanguage
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Bot.Resources.ChangeLanguage", typeof(ChangeLanguage).GetTypeInfo().Assembly);

        /// <summary>
        ///     Enter the language in which you want to speak.
        /// </summary>
        public static string ChangeLanguage_AskForLanguage
            => GetString("ChangeLanguage_AskForLanguage");

        /// <summary>
        ///     I'm able to speak English and Spanish.
        /// </summary>
        public static string ChangeLanguage_AskForLanguageRetryPrompt
            => GetString("ChangeLanguage_AskForLanguageRetryPrompt");

        /// <summary>
        ///     Vale hablamos en español
        /// </summary>
        public static string ChangeLanguage_ConfirmChange
            => GetString("ChangeLanguage_ConfirmChange");

        /// <summary>
        ///     Do you want to change language to {language}?
        /// </summary>
        public static string ChangeLanguage_Question(object language)
            => string.Format(
                GetString("ChangeLanguage_Question", nameof(language)),
                language);

        /// <summary>
        ///     English
        /// </summary>
        public static string CurrentLanguage
            => GetString("CurrentLanguage");

        /// <summary>
        ///     Cancel
        /// </summary>
        public static string GetLanguage_Cancel
            => GetString("GetLanguage_Cancel");

        /// <summary>
        ///     English
        /// </summary>
        public static string GetLanguage_ChoiceEnglish
            => GetString("GetLanguage_ChoiceEnglish");

        /// <summary>
        ///     Spanish
        /// </summary>
        public static string GetLanguage_ChoiceSpanish
            => GetString("GetLanguage_ChoiceSpanish");

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
    internal static class ChangeLanguage
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Bot.Resources.ChangeLanguage", typeof(ChangeLanguage).GetTypeInfo().Assembly);
    }
}
