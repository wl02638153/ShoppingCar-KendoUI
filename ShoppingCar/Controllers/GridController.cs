using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.UI;
using ShoppingCar.Models;

namespace ShoppingCar.Controllers
{
    public class GridController : Controller
    {
        // GET: Grid
        public ActionResult Index()
        {
            return View();
        }

        //public static class ProductCustomBindingExtensions
        //{
        //    public static IQueryable<Product> ApplyPaging(this IQueryable<Product> data, int page, int pageSize)
        //    {
        //        if (pageSize > 0 && page > 0)
        //        {
        //            data = data.Skip((page - 1) * pageSize);
        //        }

        //        data = data.Take(pageSize);

        //        return data;
        //    }
        //}
    }

    
}