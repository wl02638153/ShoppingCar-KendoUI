using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ShoppingCar.Controllers
{
    public class DisplayAttributeHelper<TModel> where TModel : class
    {
        #region -- GetDisplayName --

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static string GetDisplayName(string propertyName)
        {
            Type type = typeof(TModel);
            Type metaDataType = null;

            foreach (MetadataTypeAttribute attrib in type.GetCustomAttributes(typeof(MetadataTypeAttribute), true))
            {
                metaDataType = attrib.MetadataClassType;
            }

            if (metaDataType == null)
            {
                return propertyName;
            }

            PropertyInfo pInfo = GetProperty(type, propertyName);
            return DisplayAttributeHelper<TModel>.GetDisplayName(pInfo, metaDataType);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <returns></returns>
        private static PropertyInfo GetProperty(Type type, string propName)
        {
            try
            {
                PropertyInfo[] infos = type.GetProperties();
                if (infos == null)
                {
                    return null;
                }
                foreach (PropertyInfo info in infos)
                {
                    if (propName.ToLower().Equals(info.Name.ToLower()))
                    {
                        return info;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
            return null;
        }

        /// <summary>
        /// Gets the property Display Name.
        /// </summary>
        /// <param name="pInfo">The p info.</param>
        /// <returns></returns>
        public static string GetDisplayName(PropertyInfo pInfo, Type metaDataType)
        {
            if (null == pInfo)
            {
                return String.Empty;
            }

            string propertyName = pInfo.Name;

            DisplayAttribute attr = (DisplayAttribute)metaDataType.GetProperty(propertyName)
                .GetCustomAttributes(typeof(DisplayAttribute), true)
                .SingleOrDefault();

            if (attr == null)
            {
                MetadataTypeAttribute otherType =
                    (MetadataTypeAttribute)metaDataType.GetCustomAttributes(typeof(MetadataTypeAttribute), true)
                    .FirstOrDefault();

                if (otherType != null)
                {
                    var property = otherType.MetadataClassType.GetProperty(propertyName);
                    if (property != null)
                    {
                        attr = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    }
                }
            }
            return (attr != null) ? attr.Name : String.Empty;
        }

        #endregion

    }
}