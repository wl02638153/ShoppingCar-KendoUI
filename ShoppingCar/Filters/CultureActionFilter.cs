using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCar.Filters
{
    public class CultureActionFilter : ActionFilterAttribute
    {
        private string culture;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.RouteData.Values.ContainsKey("culture"))
            {
                culture = filterContext.RouteData.Values["culture"].ToString();
            }
            else
                culture = "en-US";

            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }

    }
}