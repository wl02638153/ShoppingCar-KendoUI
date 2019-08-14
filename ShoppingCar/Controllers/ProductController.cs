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

namespace ShoppingCar.Controllers
{
    public class ProductController : Controller
    {
        dbShoppingCarEntities3 db = new dbShoppingCarEntities3();     //存取db
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        // GET: Product
        public ActionResult Index()
        {
            var products = db.Product.ToList();

            if (Session["Member"] == null)
            {
                return View("Index", "_Layout", products);
            }
            else if (Session["Welcome"].ToString() == "Admin歡迎光臨")
            {
                return View("Index", "_LayoutAdmin", products);
            }
            return View("Index", "_LayoutMember", products);
        }

        //[Authorize]
        public ActionResult CreateProduct()
        {
            return View("CreateProduct", "_LayoutAdmin");
        }
        [HttpPost]
        [CreateProductFilter]
        public ActionResult CreateProduct(Product cProduct, string base64str)
        {
            if (base64str != null && base64str.Length > 0)
            {
                //local
                string fileName = cProduct.ProductID + ".PNG";
                var path = Path.Combine(Server.MapPath("~/Image"), fileName);
                //Base64ToImage(base64str).Save(path);
                base64str = base64str.Replace("data:image/jpeg;base64,", "");
                byte[] imageBytes = Convert.FromBase64String(base64str);
                MemoryStream mss = new MemoryStream(imageBytes, 0, imageBytes.Length);
                mss.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(mss, true);
                image.Save(path);
                cProduct.ProductImg = "~/Image/" + fileName;

                //db
                cProduct.ProductImg_DB = imageBytes;

                cProduct.Create_Date = DateTime.Now;
                cProduct.Delete_Flag = false;
                db.Product.Add(cProduct);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return View("CreateProduct", "_LayoutAdmin");
        }

        [HttpPost]  //excel insert
        public ActionResult ImportProduct(HttpPostedFileBase ImportFile)
        {
            ExcelPackage ep = new ExcelPackage(ImportFile.InputStream);
            var workbook = ep.Workbook;
            if (workbook != null)
            {
                if (workbook.Worksheets.Count > 0)
                {
                    var currentWorkSheet = workbook.Worksheets.First();
                    object colHeader = currentWorkSheet.Cells[2, 2].Value;
                    int col = 1;
                    int row = 3;
                    foreach (var item in currentWorkSheet.Cells)
                    {
                        if (currentWorkSheet.Cells[row, col].Value != null)
                        {
                            Product product = new Product();
                            product.ProductID = currentWorkSheet.Cells[row, col++].Value.ToString();
                            product.ProductName = currentWorkSheet.Cells[row, col++].Value.ToString();
                            product.ProductExplain = currentWorkSheet.Cells[row, col++].Value.ToString();
                            product.ProductPrice = Convert.ToDecimal((double)currentWorkSheet.Cells[row, col++].Value);
                            product.Create_Date = DateTime.Now;
                            product.Delete_Flag = false;
                            byte[] temp = BitConverter.GetBytes(0);
                            product.ProductImg_DB = temp;
                            db.Product.Add(product);
                            col = 1;
                            row++;

                        }
                        else
                        {
                            break;
                        }
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("ProductList");
        }

        public ActionResult ProductList()
        {
            var products = db.Product.ToList<Product>();
            return View("ProductList", "_LayoutAdmin", products);
        }

        public ActionResult ProductEdit(string ProductID)
        {
            var Product = db.Product.Where(m => m.ProductID == ProductID).FirstOrDefault();
            return View("ProductEdit", "_LayoutAdmin", Product);
        }

        [HttpPost]
        public ActionResult ProductEdit(Product product, string base64str)   //string ProductExplain,string ProductName,decimal ProductPrice, string ProductID
        {
            var Product = db.Product.Where(m => m.ProductID == product.ProductID).FirstOrDefault();
            if (base64str != null && base64str.Length > 0)
            {
                //local
                string fileName = product.ProductID + ".PNG";
                var path = Path.Combine(Server.MapPath("~/Image"), fileName);
                //Base64ToImage(base64str).Save(path);
                base64str = base64str.Replace("data:image/jpeg;base64,", "");
                byte[] imageBytes = Convert.FromBase64String(base64str);
                MemoryStream mss = new MemoryStream(imageBytes, 0, imageBytes.Length);
                mss.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(mss, true);
                image.Save(path);
                Product.ProductImg = "~/Image/" + fileName;

                //db
                Product.ProductImg_DB = imageBytes;

                Product.Modify_Date = DateTime.Now;
                Product.ProductExplain = product.ProductExplain;
                Product.ProductName = product.ProductName;
                Product.ProductPrice = product.ProductPrice;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            db.SaveChanges();
            return RedirectToAction("ProductList");
        }

        public ActionResult DownloadProductExcel()
        {
            var products = db.Product.ToList();
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("FirstSheet");
            int col = 1;
            sheet.Cells[1, col++].Value = "產品編號";
            sheet.Cells[1, col++].Value = "產品名稱";
            sheet.Cells[1, col++].Value = "產品說明";
            sheet.Cells[1, col++].Value = "價錢";
            int row = 2;
            foreach (Product item in products)
            {
                col = 1;
                sheet.Cells[row, col++].Value = item.ProductID;
                sheet.Cells[row, col++].Value = item.ProductName;
                sheet.Cells[row, col++].Value = item.ProductExplain;
                sheet.Cells[row, col++].Value = item.ProductPrice;
                row++;
            }
            MemoryStream ms = new MemoryStream();
            ep.SaveAs(ms);
            ep.Dispose();
            ms.Position = 0;
            string filename = "AllProductList.xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        public ActionResult DownloadProductExcel2()
        {
            var products = db.Product.ToList();
            string rng = "D" + (products.Count + 1);  //excel col
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("FirstSheet");
            ExcelTableCollection tblcollection = sheet.Tables;
            ExcelTable table = tblcollection.Add(sheet.Cells["A1:" + rng], "Product");

            //get product attribute
            var t = typeof(ProductMetaData);
            int col = 1;
            table.Columns[0].Name = t.GetProperty("ProductID").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[1].Name = t.GetProperty("ProductName").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[2].Name = t.GetProperty("ProductExplain").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[3].Name = t.GetProperty("ProductPrice").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.ShowFilter = true;
            table.ShowTotal = true;
            int row = 2;
            foreach (Product item in products)
            {
                col = 1;

                sheet.Cells[row, col++].Value = item.ProductID;
                sheet.Cells[row, col++].Value = item.ProductName;
                sheet.Cells[row, col++].Value = item.ProductExplain;
                sheet.Cells[row, col++].Value = item.ProductPrice;
                row++;
            }
            MemoryStream ms = new MemoryStream();
            ep.SaveAs(ms);
            ep.Dispose();
            ms.Position = 0;
            string filename = "AllProductList.xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
    }
}