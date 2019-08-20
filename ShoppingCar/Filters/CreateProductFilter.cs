using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using NLog;
using System.Web.Routing;
using ShoppingCar.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace ShoppingCar.Filters
{
    public class CreateProductFilter : ActionFilterAttribute, IActionFilter
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //dbShoppingCarEntities3 db = new dbShoppingCarEntities3();     //存取db
        ShoppingCartEntities db = new ShoppingCartEntities();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string UserID = filterContext.HttpContext.Session["Member"].ToString();
            string parameter = JsonConvert.SerializeObject(filterContext.ActionParameters, new JsonSerializerSettings()
            {
                ContractResolver = new ReadablePropertiesOnlyResolver()
            });
            WriteLog("OnActionExecuting", filterContext.RouteData,UserID,parameter);
            //OnActionExecuting(filterContext);
        }
        public void WriteLog(string methodName, RouteData routeData,string UserID,string parameter)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            logger.Info("-----------" + methodName + "-----------");
            logger.Info("UserID : " + UserID);
            logger.Info("Date Time : " + DateTime.Now);
            LogMessage log = new LogMessage();
            log.ActionID = actionName.ToString();
            log.UserID = UserID;
            log.Date = DateTime.Now;
            log.Message = string.Format("{0}.{1}() => {2}", controllerName, actionName, string.IsNullOrEmpty(parameter) ? "(void)" : parameter);
            logger.Info("Message : " + log.Message);
            db.LogMessage.Add(log);
            db.SaveChanges();
        }
    }

    class ReadablePropertiesOnlyResolver : DefaultContractResolver
    {
        /// <summary>
        /// 建立可呈現（解析）的屬性
        /// </summary>
        /// <returns>呈現的屬性</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (typeof(Stream).IsAssignableFrom(property.PropertyType))
            {
                property.Ignored = true;
            }
            return property;
        }
    }
}