using ImageMagick;
using ShoppingCar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ShoppingCar.Service
{
    public class ProductService : IDisposable
    {
        private static bool UpdateDatabase = false;
        private ShoppingCartEntities db;

        public ProductService(ShoppingCartEntities db)
        {
            this.db = db;
        }

        public IList<Product> GetAll()
        {
            var result = HttpContext.Current.Session["Products"] as IList<Product>;
            if (result == null || UpdateDatabase)
            {
                
                result = db.Product.ToList().Select(product => new Product
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
                HttpContext.Current.Session["Products"] = result;
            }
            UpdateDatabase = false;
            return result;
        }

        public IEnumerable<Product> Read()
        {
            return GetAll();
        }

        public void Destroy(string ProductID)
        {
            if (!UpdateDatabase)
            {
                var result = GetAll().FirstOrDefault(p => p.ProductID == ProductID);
                if (result != null)
                {
                    GetAll().Remove(result);
                    UpdateDatabase = true;
                }
            }
            else
            {
                var p = new Product();
                p.ProductID = ProductID;
                db.Product.Attach(p);
                db.Product.Remove(p);
                db.SaveChanges();
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}