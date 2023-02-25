using Bot.Resources;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Globalization;
using System.Reflection;

namespace Bot.Dialogs
{
    public class DialogHelper
    {

        public static Dictionary<string, (Choice, Choice, ChoiceFactoryOptions)> InitializeConfirmChoiceDictionary()
        {
            System.Resources.ResourceManager resourceManager
            = new System.Resources.ResourceManager("Bot.Resources.Confirm", typeof(Common).GetTypeInfo().Assembly);

            CultureInfo? cultureSpanish = new CultureInfo("es");
            CultureInfo? cultureEnglish = new CultureInfo("en");

            var customDictionary = new Dictionary<string, (Choice, Choice, ChoiceFactoryOptions)>()
            {
                    { Common.English,
                                        (
                                        new Choice(resourceManager.GetString("Yes", cultureEnglish)),
                                        new Choice(resourceManager.GetString("No", cultureEnglish)),
                                        new ChoiceFactoryOptions(Confirm.PromptCultureModel_Separator, resourceManager.GetString("PromptCultureModel_InlineOr",cultureEnglish),  Confirm.PromptCultureModel_InlineOrMore, false)
                                        )
                    },

                    { Common.Spanish,
                                        (
                                        new Choice(resourceManager.GetString("Yes", cultureSpanish)),
                                        new Choice(resourceManager.GetString("No", cultureSpanish)),
                                        new ChoiceFactoryOptions(Confirm.PromptCultureModel_Separator, resourceManager.GetString("PromptCultureModel_InlineOr",cultureSpanish),  Confirm.PromptCultureModel_InlineOrMore, false)
                                        )
                    }
            };
            return customDictionary;
        }
    }
}