using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using NLog;
using System.Web.Routing;

namespace ShoppingCar.Filters
{
    public class MemberFilter : ActionFilterAttribute, IActionFilter
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var controllerName=
            WriteLog("OnActionExecuting", filterContext.RouteData,filterContext.HttpContext.Request.Url);
            if (filterContext.HttpContext.Session["Member"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new 
                    RouteValueDictionary(new
                    {
                        action = "Login",
                        controller = "Member",
                        returnUrl = filterContext.HttpContext.Request.Url
                    }));
            }
        }
        public void WriteLog(string methodName, RouteData routeData,System.Uri url)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            logger.Info("-----------" + methodName + "-----------");
            logger.Info("Controller : " + controllerName);
            logger.Info("Action : " + actionName);
            logger.Info("ReturnUrl=" + url);
        }
    }
}