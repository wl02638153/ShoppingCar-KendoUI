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
using System.Data.Entity.Validation;
using PagedList;
using System.Diagnostics;
using System.Data.Entity;
using ShoppingCar.Service;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Kendo.Mvc;

namespace ShoppingCar.Controllers
{
    [Filters.MemberFilter]
    public class ProductController : Controller
    {
        //dbShoppingCarEntities3 db = new dbShoppingCarEntities3();     //存取db

        ShoppingCartEntities db = new ShoppingCartEntities();
        
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        ProductService productService = new ProductService(new ShoppingCartEntities());


        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateProduct()
        {
            return View("CreateProduct");
        }
        [HttpPost]
        [CreateProductFilter]
        public ActionResult CreateProduct(HttpPostedFileBase ImgFile, Product cProduct, string base64str)
        {
            //驗證檔案
            ImgUploadValidate ImgV = new ImgUploadValidate();
            ImgV.filesize = 2000;//限制2mb
            if (ImgV.UploadUserFile(ImgFile))
            {//檔案驗證成功
                //local
                string fileName = cProduct.ProductID + ImgV.contentType;
                var path = Path.Combine(Server.MapPath("~/Image"), fileName);
                ImgFile.SaveAs(path);
                cProduct.ProductImg = "~/Image/" + fileName;

                //compress
                using (ImageMagick.MagickImage oImage = new ImageMagick.MagickImage(path))
                {
                    oImage.Format = MagickFormat.Jpg;
                    oImage.ColorSpace = ImageMagick.ColorSpace.sRGB;  //色盤採用sRGB
                    oImage.Quality = 80;    //壓縮率
                    if (oImage.Height < 200)
                        oImage.Resize(0, 200);
                    oImage.Strip(); //去除圖片profile
                    oImage.Write(path);
                }
                //db
                byte[] FileBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    ImgFile.InputStream.CopyTo(ms);
                    FileBytes = ms.GetBuffer();
                }
                cProduct.ProductImg_DB = FileBytes;
                cProduct.Create_Date = DateTime.Now;
                cProduct.Delete_Flag = false;
                cProduct.Shelf_Flag = true;
                if (ModelState.IsValid)
                {//model驗證成功
                    if (db.Product.Any(p => p.ProductID.Equals(cProduct.ProductID)))    //判斷資料是否重複
                    {
                        TempData["DBResultErrorMessage"] = cProduct.ProductID + "資料已重複，請重新輸入";
                        return View("CreateProduct");
                    }
                    try
                    {
                        db.Product.Add(cProduct);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {//資料庫異動例外狀況
                        logger.Error(ex.Message);
                        TempData["DBResultErrorMessage"] = ex.Message;
                        return View("CreateProduct");
                    }
                    TempData["DBResultErrorMessage"] = cProduct.ProductID + "新增成功!";
                    return RedirectToAction("ProductList");    //新增成功
                }
                else
                {   //model 驗證失敗
                    return View("CreateProduct");
                }
            }
            else
            {//檔案驗證失敗
                TempData["ImgValidate"] = ImgV.ErrorMessage;
                return View("CreateProduct");
            }
        }

        [HttpPost]
        [CreateProductFilter]
        public ActionResult ImportProduct(HttpPostedFileBase ImportFile)
        {
            //check file format
            FileUploadValidate fs = new FileUploadValidate();
            fs.filesize = 2000;
            ExcelValidate ev = new ExcelValidate();
            string message = "";
            if (fs.UploadUserFile(ImportFile))  //判斷檔案是否合法
            {
                if (ev.CheckExcelData(ImportFile))//判斷excel是否有內容
                {
                    var currentWorkSheet = ev.workbook.Worksheets.First();
                    if (currentWorkSheet.Cells[102, 1].Value != null)
                    {
                        message = "最多上傳100筆產品";
                        TempData["ExcelResultMessage"] = message;
                        return View("CreateProduct");
                    }
                    int col = 1;
                    int row = 2;
                    DateTime time_start = DateTime.Now;
                    ShoppingCartEntities dbn = new ShoppingCartEntities();
                    foreach (var item in currentWorkSheet.Cells)
                    {
                        Product product = new Product();
                        if (currentWorkSheet.Cells[row, col].Value != null)
                        {
                            try
                            {
                                product.ProductID = currentWorkSheet.Cells[row, col++].Value.ToString();
                                if (db.Product.Any(p => p.ProductID.Equals(product.ProductID)))    //判斷資料是否重複
                                {
                                    message += "<p>[第" + row + "列]" + product.ProductID + "資料已重複<p/>";   //ViewBag.DBResultErrorMessage
                                    continue;
                                }
                                product.ProductName = currentWorkSheet.Cells[row, col++].Value.ToString();
                                product.ProductExplain = currentWorkSheet.Cells[row, col++].Value.ToString();
                                product.ProductPrice = Convert.ToDecimal((double)currentWorkSheet.Cells[row, col++].Value);
                                product.Create_Date = DateTime.Now;
                                product.Delete_Flag = false;
                                product.Shelf_Flag = true;
                                byte[] temp = BitConverter.GetBytes(0);
                                product.ProductImg_DB = temp;

                                
                                db.Product.Add(product);
                                db.SaveChanges();
                                message += "<p>[第" + row + "列]" + product.ProductID + "上傳成功 <p/>";
                            }
                            catch (DbEntityValidationException ex)
                            {
                                logger.Error(ex.Message);
                                TempData["ExcelResultErrorMessage"] = "請確認資料格式是否正確";
                                foreach (var err in ex.EntityValidationErrors)
                                {
                                    foreach (var erro in err.ValidationErrors)
                                    {
                                        var ErrID = erro.PropertyName;
                                        if (ErrID == "ProductID") ErrID = "產品編號";
                                        else if (ErrID == "ProductName") ErrID = "產品名稱";
                                        else if (ErrID == "ProductExplain") ErrID = "產品說明";
                                        else if (ErrID == "ProductPrice") ErrID = "產品價錢";
                                        message += "<p>[第" + row + "列," + ErrID + "]" + erro.ErrorMessage + "<p/>";
                                    }
                                }
                                db.Product.Remove(product); //移除錯誤實體避免判斷錯誤
                            }
                            catch (InvalidCastException ex)
                            {
                                var ErrID = "";
                                if ((col - 1) == 1) ErrID = "產品編號";
                                else if ((col - 1) == 2) ErrID = "產品名稱";
                                else if ((col - 1) == 3) ErrID = "產品說明";
                                else if ((col - 1) == 4) ErrID = "產品價錢";
                                message += "<p>[" + row + "," + ErrID + "]資料型態輸入錯誤</p>";
                            }
                            catch (Exception ex)
                            {
                                TempData["ExcelResultErrorMessage"] = ex.Message;
                            }
                            finally
                            {
                                col = 1;
                                row++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    //db.SaveChanges();
                    //db.Dispose();
                    DateTime time_end = DateTime.Now;
                    TempData["ExcelInsertTime"] = "共花了" + (((TimeSpan)(time_end - time_start)).TotalMilliseconds / 1000).ToString() + "秒";
                    TempData["ExcelResultMessage"] = message;
                }
                else
                {
                    TempData["ExcelResultMessage"] = ev.ErrorMessage;
                }
            }
            else
            {//檔案驗證失敗
                TempData["ExcelResultErrorMessage"] = fs.ErrorMessage;
            }

            return View("CreateProduct");
        }

        public ActionResult ProductList(int page = 1)
        {
            int pageSize = 10;
            int currentPage = page < 1 ? 1 : page;
            var products = db.Product.Where(m => m.Delete_Flag == false).OrderByDescending(m => m.Create_Date).ToList();
            var product = products.ToPagedList(currentPage, pageSize);
            return View("ProductList", product);
        }

        public ActionResult ProductEdit(string ProductID)
        {
            Product Product = db.Product.Find(ProductID);
            var categorys = db.Product_Category.ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach(var category in categorys)
            {
                items.Add(new SelectListItem()
                {
                    Text = category.CategoryName,
                    Value=category.CategoryID.ToString()
                });
            }
            ViewBag.CategoryItems = items;

            return View("ProductEdit", Product);
        }
        [HttpPost]
        public ActionResult ProductEdit(Product product, HttpPostedFileBase ImgFile)
        {
            var Product = db.Product.Where(m => m.ProductID == product.ProductID).FirstOrDefault();
            //驗證檔案
            ImgUploadValidate ImgV = new ImgUploadValidate();
            ImgV.filesize = 2000;//限制2mb
            if (ImgV.UploadUserFile(ImgFile) || ImgV.ErrorMessage == "檔案不能為空的")
            {//檔案驗證成功
                if (ImgV.UploadUserFile(ImgFile))
                {
                    //local

                    string fileName = product.ProductID + ImgV.contentType;
                    var path = Path.Combine(Server.MapPath("~/Image"), fileName);
                    ImgFile.SaveAs(path);
                    product.ProductImg = "~/Image/" + fileName;

                    //compress
                    using (ImageMagick.MagickImage oImage = new ImageMagick.MagickImage(path))
                    {
                        oImage.Format = MagickFormat.Jpg;
                        oImage.ColorSpace = ImageMagick.ColorSpace.sRGB;  //色盤採用sRGB
                        oImage.Quality = 80;    //壓縮率
                        if (oImage.Height < 200)
                            oImage.Resize(0, 200);
                        oImage.Strip(); //去除圖片profile
                        oImage.Write(path);
                    }

                    //db
                    byte[] FileBytes;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ImgFile.InputStream.CopyTo(ms);
                        FileBytes = ms.GetBuffer();
                    }
                    Product.ProductImg_DB = FileBytes;
                    Product.ProductImg = "~/Image/" + fileName;
                }
                Product.Modify_Date = DateTime.Now;
                Product.ProductExplain = product.ProductExplain;
                Product.ProductName = product.ProductName;
                Product.ProductPrice = product.ProductPrice;
                Product.Shelf_Flag = product.Shelf_Flag;
                Product.CategoryID = product.CategoryID;

                try
                {
                    if (ModelState.IsValid)
                    {   //model驗證成功
                        db.SaveChanges();
                        TempData["EditMessage"] = product.ProductID + "更新成功!";
                        return RedirectToAction("ProductList");
                    }
                    else
                    {   //model驗證失敗
                        TempData["EditMessage"] = "更新失敗，請確認資料格式是否正確!!";
                        return View(Product);
                    }
                }
                catch (Exception ex)
                {   //資料庫例外狀況
                    logger.Error(ex.Message);
                    TempData["EditMessage"] = ex.Message;
                    return View(Product);
                }
                //return RedirectToAction("ProductList");
            }
            else
            {   //檔案驗證失敗
                TempData["EditMessage"] = ImgV.ErrorMessage;
                return View(Product);
            }
        }

        public System.Drawing.Image Base64ToImage(string base64str)
        {

            byte[] imageBytes = Convert.FromBase64String(base64str);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
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

        public ActionResult DeleteProduct(string ProductID)
        {
            var Product = db.Product.Where(m => m.ProductID == ProductID).FirstOrDefault();
            //Product.Delete_Flag = true;
            if (ModelState.IsValid)
            {
                db.Product.Remove(Product);
                db.SaveChanges();
                TempData["DeleteMessage"] = Product.ProductID + "刪除成功";
            }
            return RedirectToAction("ProductList");
        }

        

        public ActionResult InsertProductTest()
        {
            var count = 100000;
            var batchCount = 100;
            int InsertCount = 0;
            string message = "";
            int j = 1;
            DateTime time_start = DateTime.Now;
            ShoppingCartEntities dbn = null;
            for (int i = 0; i < (count / batchCount); i++)
            {
                using (dbn = new ShoppingCartEntities())
                {
                    dbn.Configuration.AutoDetectChangesEnabled = false;
                    for ( j = 1; j <= batchCount; j++)
                    {
                        Product product = new Product();
                        product.ProductID = "P" + (i * batchCount + j);
                        product.ProductName = "P" + (i * batchCount + j);
                        product.ProductExplain = "P" + (i * batchCount + j);
                        product.ProductPrice = 1000;
                        product.Create_Date = DateTime.Now;
                        product.Delete_Flag = false;
                        product.Shelf_Flag = true;
                        product.ProductImg_DB = BitConverter.GetBytes(0);
                        dbn.Product.Add(product);
                        InsertCount++;
                    }
                    dbn.SaveChanges();
                }
            }
            DateTime time_end = DateTime.Now;
            TempData["InsertProductTest"] = "新增了"+ InsertCount + "筆資料共花了" + (((TimeSpan)(time_end - time_start)).TotalMilliseconds / 1000).ToString() + "秒";
            logger.Info("Excel Insert Product:" + "新增了" + InsertCount + "筆資料共花了" + (((TimeSpan)(time_end - time_start)).TotalMilliseconds / 1000).ToString() + "秒");
            TempData["InsertProductTestMessage"]= message;
            return RedirectToAction("CreateProduct");
        }

        public ShoppingCartEntities InsertProductContext(ShoppingCartEntities context, Product product ,int count,int commitCount,bool recreateContext)
        {
            context.Set<Product>().Add(product);
            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new ShoppingCartEntities();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }
            return context;
        }

        [HttpPost]
        [CreateProductFilter]
        public ActionResult ImportLargeProduct(HttpPostedFileBase ImportLargeFile)
        {
            //check file format
            FileUploadValidate fs = new FileUploadValidate();
            fs.filesize = 2000;
            ExcelValidate ev = new ExcelValidate();
            string message = "";
            if (fs.UploadUserFile(ImportLargeFile))  //判斷檔案是否合法
            {
                if (ev.CheckExcelData(ImportLargeFile))//判斷excel是否有內容
                {
                    var currentWorkSheet = ev.workbook.Worksheets.First();
                    int col = 1;
                    int row = 2;
                    int count = 0;
                    DateTime time_start = DateTime.Now;
                    ShoppingCartEntities dbn = null;
                    try
                    {
                        dbn = new ShoppingCartEntities();
                        dbn.Configuration.AutoDetectChangesEnabled = false;

                        foreach (var item in currentWorkSheet.Cells)
                        {
                            ++count;
                            Product product = new Product();
                            if (currentWorkSheet.Cells[row, col].Value != null)
                            {
                                product.ProductID = currentWorkSheet.Cells[row, col++].Value.ToString();
                                product.ProductName = currentWorkSheet.Cells[row, col++].Value.ToString();
                                product.ProductExplain = currentWorkSheet.Cells[row, col++].Value.ToString();
                                product.ProductPrice = Convert.ToDecimal((double)currentWorkSheet.Cells[row, col++].Value);
                                product.Create_Date = DateTime.Now;
                                product.Delete_Flag = false;
                                product.Shelf_Flag = true;
                                byte[] temp = BitConverter.GetBytes(0);
                                product.ProductImg_DB = temp;

                                dbn = InsertProductContext(dbn, product, count, 100, true);
                                col = 1;
                                row++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        dbn.SaveChanges();
                    }
                    finally
                    {
                        if (dbn != null)
                            dbn.Dispose();
                    }
                    DateTime time_end = DateTime.Now;
                    TempData["ExcelLargeInsertTime"] = "新增了"+(count-1)+"筆資料共花了" + (((TimeSpan)(time_end - time_start)).TotalMilliseconds / 1000).ToString() + "秒";
                    logger.Info("Excel Insert Product:"+ "新增了" + (count - 1) + "筆資料共花了" + (((TimeSpan)(time_end - time_start)).TotalMilliseconds / 1000).ToString() + "秒");
                    TempData["ExcelLargeResultMessage"] = message;
                }
                else
                {
                    TempData["ExcelLargeResultMessage"] = ev.ErrorMessage;
                }
            }
            else
            {//檔案驗證失敗
                TempData["ExcelLargeResultErrorMessage"] = fs.ErrorMessage;
            }

            return View("CreateProduct");
        }
        
        public ActionResult Products_Read([DataSourceRequest] DataSourceRequest request)
        {
            var sort = request.Sorts;
            var category = 0;
            if(request.Filters != null)
            {
                if (request.Filters.Count > 0)
                {
                    foreach (var filter in request.Filters)
                    {
                        var descriptor = filter as FilterDescriptor;
                        if (descriptor != null && descriptor.Member == "CategoryID")
                        {
                            category = Convert.ToInt32(descriptor.ConvertedValue);
                        }
                    }
                }
            }

            if (category == 0)
            {
                var AllProducts = db.Product.OrderByDescending(product => product.Create_Date).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList().Select(product => new Product
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
                    Shelf_Flag = product.Shelf_Flag,
                    CategoryID = product.CategoryID,
                    Product_Category = new Product_Category()
                    {
                        CategoryID = product.Product_Category.CategoryID,
                        CategoryName = product.Product_Category.CategoryName
                    },
                }).ToList();
                var result = new DataSourceResult()
                {
                    Data = AllProducts,
                    Total = db.Product.Count(),

                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var AllProducts = db.Product.Where(p => p.CategoryID == category).OrderByDescending(product => product.Create_Date).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList().Select(product => new Product
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
                    Shelf_Flag = product.Shelf_Flag,
                    CategoryID = product.CategoryID,
                    Product_Category = new Product_Category()
                    {
                        CategoryID = product.Product_Category.CategoryID,
                        CategoryName = product.Product_Category.CategoryName
                    },
                }).ToList();
                var result = new DataSourceResult()
                {
                    Data = AllProducts,
                    Total = db.Product.Where(p => p.CategoryID == category).Count(),

                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
       
        public ActionResult Product_Category()
        {
            var Categorys = db.Product_Category.OrderBy(e => e.CategoryName).ToList(); ;
            //return Json(Categorys, JsonRequestBehavior.AllowGet);
            return Content(JsonConvert.SerializeObject(Categorys), "application/json");
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType,string base64,string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            return File(fileContents, contentType, fileName);
        }
        
        public ActionResult Basic_Usage_Submit(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data/file"), fileName);

                    // The files are not actually saved in this demo
                    file.SaveAs(physicalPath);
                }
            }

            return RedirectToAction("CreateProduct");
        }
    }
}