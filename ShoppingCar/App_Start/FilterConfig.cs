using System.Web;
using System.Web.Mvc;
using ShoppingCar.Filters;

namespace ShoppingCar
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new MyFilter());
        }
    }
}
