using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ShoppingCar.Helper
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum self)
        {
            FieldInfo fi = self.GetType().GetField(self.ToString());
            DescriptionAttribute[] attributes = null;

            if (fi != null)
            {
                attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                    return attributes[0].Description;
            }

            return self.ToString();
        }
    }
}