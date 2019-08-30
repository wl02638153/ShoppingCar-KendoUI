using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingCar.Models;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Newtonsoft.Json;
using System.ComponentModel;
using ShoppingCar.Filters;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ImageMagick;
using PagedList;
using PagedList.Mvc;

namespace ShoppingCar.Controllers
{

    public class HomeController : Controller
    {
        //dbShoppingCarEntities3 db = new dbShoppingCarEntities3();     //存取db
        ShoppingCartEntities db = new ShoppingCartEntities();
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        

        public ActionResult Index(int page=1)
        {
            int pageSize = 9;
            int currentPage = page < 1 ? 1 : page;
            var products = db.Product.Where(m => m.Delete_Flag == false).OrderByDescending(m => m.Create_Date).ToList();
            ViewBag.PageOfProduct = products.ToPagedList(currentPage, pageSize);
            if (Session["Member"] == null)
            {
                Session["UserTag"]= "_Layout";
                return View("Index", Session["UserTag"].ToString(), products);
            }
            else if (Session["Welcome"].ToString() == "Admin歡迎光臨")
            {
                Session["UserTag"] = "_LayoutAdmin";
                return View("Index", Session["UserTag"].ToString(), products);
            }
            Session["UserTag"] = "_LayoutMember";
            return View("Index", Session["UserTag"].ToString(), products);
        }

        

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        

    }
}