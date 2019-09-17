using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ShoppingCar.Helper
{
    public class EnumHelper
    {
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", "description");
        }

        public static bool TryGetValueFromDescription<T>(string description)
        {
            bool isOkToGetValueFromDescription = true;

            try
            {
                GetValueFromDescription<T>(description);
            }
            catch (Exception)
            { isOkToGetValueFromDescription = false; }


            return isOkToGetValueFromDescription;
        }
    }
}