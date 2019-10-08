using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ShoppingCar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ShoppingCar.Controllers
{
    public class KendoController : Controller
    {
        ShoppingCartEntities db = new ShoppingCartEntities();

        // GET: Kendo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductListView()
        {
            return View();
        }

        public PartialViewResult ProductGridView()
        {
            return PartialView();
        }

        public ActionResult Products_Read([DataSourceRequest] DataSourceRequest request)
        {
            db.Configuration.ProxyCreationEnabled = false;
            
            var AllProducts = db.Product.OrderBy(product => product.Create_Date).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList().Select(product => new Product
            {
                ID = product.ID,
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                ProductExplain = product.ProductExplain,
                ProductPrice = product.ProductPrice,
                ProductImg = product.ProductImg == null ? "Image/notImg_.jpg" : product.ProductImg.Replace("~/", ""),
                Create_Date = product.Create_Date,
                Delete_Date = product.Delete_Date,
                Delete_Flag = product.Delete_Flag,
                Modify_Date = product.Modify_Date,
                ProductImg_DB = product.ProductImg_DB,
                Shelf_Flag = product.Shelf_Flag
            }).ToList();

            
           // var r = AllProducts.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
            var result = new DataSourceResult()
            {
                Data = AllProducts,
                Total = db.Product.Count(),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Product_Delete([DataSourceRequest] DataSourceRequest request,Product product)
        {
            var getProduct = db.Product.Where(m => m.ProductID == product.ProductID).FirstOrDefault();
            if (getProduct != null)
            {
                db.Product.Remove(getProduct);
                db.SaveChanges();
            }
            
            return Json(new[] { product }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Product_Create([DataSourceRequest] DataSourceRequest request, Product product)
        {
            product.Create_Date = DateTime.Now;
            product.Delete_Flag = false;
            product.Shelf_Flag = true;
            if (ModelState.IsValid)
            {
                db.Product.Add(product);
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }



    }
}