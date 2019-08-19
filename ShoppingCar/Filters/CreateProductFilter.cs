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
            WriteLog("OnActionExecuting", filterContext.RouteData,UserID);
            //OnActionExecuting(filterContext);
        }
        public void WriteLog(string methodName, RouteData routeData,string UserID)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            logger.Info("-----------" + methodName + "-----------");
            logger.Info("Controller : " + controllerName);
            logger.Info("Action : " + actionName);
            logger.Info("UserID : " + UserID);
            LogMessage log = new LogMessage();
            log.ActionID = actionName.ToString();
            log.UserID = UserID;
            log.Date = DateTime.Now;
            db.LogMessage.Add(log);
            db.SaveChanges();
        }
    }
}