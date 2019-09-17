using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Threading;
using System.Globalization;
using System.Web.Mvc.Html;
using System.ComponentModel;

namespace ShoppingCar.Helper
{
    
    public class CultureHelper
    {
        protected HttpSessionState session;
        
        public CultureHelper(HttpSessionState httpSessionState)
        {
            session = httpSessionState;
        }

        public enum LanguageEnum
        {
            [Description("en-US")]
            English = 1,
            [Description("zh-TW")]
            Taiwan = 2,
            [Description("zh-CN")]
            Chinese = 3
        }

        public static string GetImplementedCulture(string name)
        {
            // give a default culture just in case
            string cultureName = GetDefaultCulture();

            if (EnumHelper.TryGetValueFromDescription<LanguageEnum>(name))
                cultureName = name;

            return cultureName;
        }

        public static string GetDefaultCulture()
        {
            return LanguageEnum.English.GetDescription();
        }

        public static LanguageEnum GetCurrentLanguage()
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;

            // get implemented culture name
            currentCulture = GetImplementedCulture(currentCulture);

            // get language by implemented culture name
            return EnumHelper.GetValueFromDescription<LanguageEnum>(currentCulture);
        }


    }
}