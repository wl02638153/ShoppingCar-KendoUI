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
using PagedList.Mvc;


namespace ShoppingCar.Controllers
{
    [Filters.MemberFilter]
    public class ProductController : Controller
    {
        //dbShoppingCarEntities3 db = new dbShoppingCarEntities3();     //存取db
        ShoppingCartEntities db = new ShoppingCartEntities();
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateProduct()
        {
            return View("CreateProduct", Session["UserTag"].ToString());
        }
        /*[HttpPost]
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
                db.SaveChanges();
            }
            return View("CreateProduct", "_LayoutAdmin");
        }*/
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
                string fileName = cProduct.ProductID +ImgV.contentType;
                var path = Path.Combine(Server.MapPath("~/Image"), fileName);
                ImgFile.SaveAs(path);
                cProduct.ProductImg = "~/Image/" + fileName;

                //compress
                using (ImageMagick.MagickImage oImage = new ImageMagick.MagickImage(path))
                {
                    oImage.Format = MagickFormat.Jpg;
                    oImage.ColorSpace = ImageMagick.ColorSpace.sRGB;  //色盤採用sRGB
                    oImage.Quality = 80;    //壓縮率
                    if(oImage.Height<200)
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
                if (ModelState.IsValid)
                {//model驗證成功
                    if (db.Product.Any(p => p.ProductID.Equals(cProduct.ProductID)))    //判斷資料是否重複
                    {
                        TempData["DBResultErrorMessage"] = cProduct.ProductID + "資料已重複，請重新輸入";
                        return View("CreateProduct", Session["UserTag"].ToString());
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
                        return View("CreateProduct", Session["UserTag"].ToString());
                    }
                    TempData["DBResultErrorMessage"] = cProduct.ProductID + "新增成功!";
                    var products = db.Product.Where(m => m.Delete_Flag == false).OrderByDescending(m => m.Create_Date).ToList<Product>();
                    return View("ProductList", Session["UserTag"].ToString(), products);    //新增成功
                }
                else
                {   //model 驗證失敗
                    return View("CreateProduct", Session["UserTag"].ToString());
                }
            }
            else            
            {//檔案驗證失敗
                TempData["ImgValidate"] = ImgV.ErrorMessage;
                return View("CreateProduct", Session["UserTag"].ToString());
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
                        return View("CreateProduct", Session["UserTag"].ToString());
                    }
                    int col = 1;
                    int row = 2;
                    foreach (var item in currentWorkSheet.Cells)
                    {
                        Product product = new Product();
                        if (currentWorkSheet.Cells[row, col].Value != null)
                        {
                            try
                            {
                                product.ProductID = currentWorkSheet.Cells[row, col++].Value.ToString();
                                product.ProductName = currentWorkSheet.Cells[row, col++].Value.ToString();
                                product.ProductExplain = currentWorkSheet.Cells[row, col++].Value.ToString();
                                product.ProductPrice = Convert.ToDecimal((double)currentWorkSheet.Cells[row, col++].Value);
                                product.Create_Date = DateTime.Now;
                                product.Delete_Flag = false;
                                byte[] temp = BitConverter.GetBytes(0);
                                product.ProductImg_DB = temp;

                                if (db.Product.Any(p => p.ProductID.Equals(product.ProductID)))    //判斷資料是否重複
                                {
                                    message += "<p>[第" + row + "列]" + product.ProductID + "資料已重複<p/>";   //ViewBag.DBResultErrorMessage
                                    continue;
                                }
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
 
            return View("CreateProduct", Session["UserTag"].ToString());
        }
        
        public ActionResult ProductList(int page=1)
        {
            int pageSize = 10;
            int currentPage = page < 1 ? 1 : page;
            var products = db.Product.Where(m => m.Delete_Flag == false).OrderByDescending(m => m.Create_Date).ToList();
            var product = products.ToPagedList(currentPage, pageSize);
            return View("ProductList", Session["UserTag"].ToString(), product);
        }

        public ActionResult ProductEdit(string ProductID)
        {
            Product Product = db.Product.Find(ProductID);
            return View("ProductEdit", Product);
        }
        [HttpPost]
        public ActionResult ProductEdit(Product product, HttpPostedFileBase ImgFile)  
        {
            var Product = db.Product.Where(m => m.ProductID == product.ProductID).FirstOrDefault();
            //驗證檔案
            ImgUploadValidate ImgV = new ImgUploadValidate();
            ImgV.filesize = 2000;//限制2mb
            if (ImgV.UploadUserFile(ImgFile)||ImgV.ErrorMessage== "檔案不能為空的")
            {//檔案驗證成功
                if(ImgV.UploadUserFile(ImgFile))
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

                try
                {
                    if (ModelState.IsValid)
                    {   //model驗證成功
                        db.SaveChanges();
                        TempData["EditMessage"] = product.ProductID+"更新成功!";
                        return RedirectToAction("ProductList");
                    }
                    else
                    {   //model驗證失敗
                        TempData["EditMessage"] = "更新失敗，請確認資料格式是否正確!!";
                        return View(product);
                    }
                }
                catch (Exception ex)
                {   //資料庫例外狀況
                    logger.Error(ex.Message);
                    TempData["EditMessage"] = ex.Message;
                    return View(product);
                }
                //return RedirectToAction("ProductList");
            }
            else
            {   //檔案驗證失敗
                TempData["EditMessage"] = ImgV.ErrorMessage;
                return View(product);
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
            Product.Delete_Flag = true;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                TempData["DeleteMessage"] = Product.ProductID+"刪除成功";
            }
            return RedirectToAction("ProductList");
        }
    }
}