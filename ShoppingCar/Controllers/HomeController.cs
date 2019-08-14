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

namespace ShoppingCar.Controllers
{
    
    public class HomeController : Controller
    {
        dbShoppingCarEntities3 db = new dbShoppingCarEntities3();     //存取db
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            var products = db.Product.ToList();

            if (Session["Member"] == null)
            {
                return View("Index","_Layout",products);
            }
            else if(Session["Welcome"].ToString()== "Admin歡迎光臨")
            {
                return View("Index", "_LayoutAdmin", products);
            }
            return View("Index", "_LayoutMember", products);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UserID,string Password)
        {
            var member = db.Member.Where(m => m.UserID == UserID && m.Password == Password).FirstOrDefault(); //取得會員並指定給member

            if (member == null)
            {
                ViewBag.Message = "帳號密碼錯誤，登入失敗，請重新輸入";
                return View();
            }
            Session["Welcome"] = member.MemberName + "歡迎光臨";
            Session["Member"] = member;
            TempData["LoginMessage"] = "登入成功";
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Member rMember)
        {
            if (ModelState.IsValid == false)    //model 驗證
            {
                return View();
            }
            var member = db.Member.Where(m => m.UserID == rMember.UserID).FirstOrDefault();     //if member ==null 代表未註冊
            if(member == null)
            {
                rMember.Create_Date = DateTime.Now;
                rMember.Delete_Flag = false;
                db.Member.Add(rMember);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            ViewBag.Message = "此帳號已有人使用，註冊失敗";
            return View();
        }

        //[Authorize]
        public ActionResult CreateProduct()
        {
            return View("CreateProduct", "_LayoutAdmin");
        }
        [HttpPost]
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

        [HttpPost]
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
                    foreach(var item in currentWorkSheet.Cells)
                    {
                        if (currentWorkSheet.Cells[row, col].Value != null)
                        {
                            Product product = new Product();
                            product.ProductID= currentWorkSheet.Cells[row, col++].Value.ToString();
                            product.ProductName= currentWorkSheet.Cells[row, col++].Value.ToString();
                            product.ProductExplain= currentWorkSheet.Cells[row, col++].Value.ToString();
                            product.ProductPrice= Convert.ToDecimal((double)currentWorkSheet.Cells[row, col++].Value);
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

        [HttpPost]
        public ActionResult ImportOrder(HttpPostedFileBase ImportOrder)
        {
            ExcelPackage ep = new ExcelPackage(ImportOrder.InputStream);
            var workbook = ep.Workbook;
            if (workbook != null)
            {
                if (workbook.Worksheets.Count > 0)
                {
                    var currentWorkSheet = workbook.Worksheets.First();

                    int col = 1;
                    int row = 2;
                    foreach (var item in currentWorkSheet.Cells)
                    {
                        if (currentWorkSheet.Cells[row, col].Value != null)
                        {
                            string ProductID=currentWorkSheet.Cells[row, col].Value.ToString();

                            Add_car(ProductID);
                            row++;

                        }
                        else
                        {
                            break;
                        }
                        //db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("CheckCar");
        }

        //[Authorize]
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

        public System.Drawing.Image Base64ToImage(string base64str)
        {
            
            byte[] imageBytes = Convert.FromBase64String(base64str);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }
        /*[HttpPost]
        public ActionResult CreateProduct(HttpPostedFileBase ImgFile, Product cProduct, string base64str)
        {
            
            if (ImgFile != null && ImgFile.ContentLength > 0) 
            {
                //local
                string fileName = cProduct.ProductID+".PNG";
                var path = Path.Combine(Server.MapPath("~/Image"), fileName);
                ImgFile.SaveAs(path);
                cProduct.ProductImg = "~/Image/" + fileName;

                //db
                byte[] FileBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    ImgFile.InputStream.CopyTo(ms);
                    FileBytes = ms.GetBuffer();
                }
                cProduct.ProductImg_DB = FileBytes;
            }
            cProduct.Create_Date = DateTime.Now;
            cProduct.Delete_Flag = false;
            db.Product.Add(cProduct);
            try
            {
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                throw;
            }

            return View("CreateProduct", "_LayoutAdmin");
        }*/

        public ActionResult ShoppingCar()
        {
            string UserID = (Session["Member"] as Member).UserID;
            var orderDetails = db.OrderDetail.Where(m => m.UserID==UserID&&m.Approved_Flag==true).ToList();
            return View("ShoppingCar", "_LayoutMember", orderDetails);
        }
        [HttpPost]
        public ActionResult ShoppingCar(string Receiver,string Email,string Address)    //產生orderHeader
        {
            string UserID = (Session["Member"] as Member).UserID;
            string guid = Guid.NewGuid().ToString();        //產生唯一的guid變數 for OrderID

            OrderHeader orderHeader = new OrderHeader();
            orderHeader.OrderID = guid;
            orderHeader.UserID = UserID;
            orderHeader.Receiver = Receiver;
            orderHeader.Email = Email;
            orderHeader.Address = Address;
            orderHeader.Create_Date = DateTime.Now;
            db.OrderHeader.Add(orderHeader);
            var carList = db.OrderDetail.Where(m => m.Approved_Flag == true && m.UserID == UserID).ToList();

            foreach(var item in carList)
            {
                item.OrderID = guid;
                item.Approved_Flag = true;
            }
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }

        public ActionResult CheckCar()
        {
            /*string UserID = (Session["Member"] as Member).UserID;
            var orderDetails = db.OrderDetail.Where(m => m.UserID == UserID && m.Approved_Flag == false).ToList();
            return View("CheckCar", "_LayoutMember", orderDetails);*/
            string UserID = (Session["Member"] as Member).UserID;
            OrderDetailList detailList = new OrderDetailList();
            detailList.OrderDetails = db.OrderDetail.Where(m => m.UserID == UserID && m.OrderID == null).ToList<OrderDetail>();
            return View("CheckCar", "_LayoutMember", detailList);
        }
        [HttpPost]
        public ActionResult CheckCar(OrderDetailList list)
        {
            string UserID = (Session["Member"] as Member).UserID;
            var orderDetails = db.OrderDetail.Where(m => m.UserID == UserID && m.Approved_Flag == false).ToList<OrderDetail>();
            var selected = list.OrderDetails.ToList<OrderDetail>();
            /*var q = from od in orderDetails
                    join s in selected on od.OrderDetailID equals s.OrderDetailID into check
                    select new
                    {
                        od.Approved_Flag
                    };
            foreach(var item in q)
            {
                item.Approved_Flag = true;
            }*/

            foreach (var item in selected)
            {
                var check = db.OrderDetail.Where(m => m.OrderDetailID == item.OrderDetailID).FirstOrDefault();
                if (item.Approved_Flag == true)
                {
                    check.Approved_Flag = true;
                }
                else if(item.Approved_Flag == false)
                {
                    check.Approved_Flag = false;
                }
                
            }
            db.SaveChanges();

            var orderDetailsNew = db.OrderDetail.Where(m => m.UserID == UserID && m.Approved_Flag == true&&m.Delete_Flag!=true&&m.OrderID==null).ToList();
            //return View("ShoppingCar", "_LayoutMember", orderDetailsNew);
            return RedirectToAction("ShoppingCar", "Home", orderDetailsNew);
        }

        public ActionResult OrderList()
        {
            string UserID = (Session["Member"] as Member).UserID;
            var orders = db.OrderHeader.Where(m => m.UserID == UserID && m.Delete_Flag != true).OrderByDescending(m => m.Create_Date).ToList();  //取出order依照create date排序
            return View("OrderList","_LayoutMember",orders);
        }

        public ActionResult OrderListDetail(string OrderID)
        {
            var orderDetails = db.OrderDetail.Where(m => m.OrderID == OrderID&&m.Delete_Flag!=true).ToList();
            return View("OrderListDetail", "_LayoutMember", orderDetails);
        }

        public ActionResult DownloadOrderExcel(string OrderID)
        {
            string UserID = (Session["Member"] as Member).UserID;
            var orders = db.OrderHeader.Where(m => m.UserID == UserID && m.OrderID == OrderID).FirstOrDefault();
            var orderDetails = db.OrderDetail.Where(m => m.OrderID == OrderID && m.Delete_Flag != true).ToList();

            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("FirstSheet");
            string rng = "J" + (orderDetails.Count + 1 +1);  //excel col    //array+1  orders+1
            ExcelTableCollection tblcollection = sheet.Tables;
            ExcelTable table = tblcollection.Add(sheet.Cells["A1:" + rng], "Order");

            var tOH = typeof(OrderHeaderMetaData);
            var tOD = typeof(OrderDetailMetaData);

            int col = 1;    //欄:直的，因為要從第1欄開始，所以初始為1
            table.Columns[0].Name = tOH.GetProperty("OrderID").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[1].Name = tOH.GetProperty("Receiver").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[2].Name = tOH.GetProperty("Email").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[3].Name = tOH.GetProperty("Address").GetCustomAttribute<DisplayNameAttribute>().DisplayName;

            table.Columns[4].Name = tOD.GetProperty("ProductID").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[5].Name = tOD.GetProperty("ProductName").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[6].Name = tOD.GetProperty("UserID").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[7].Name = tOD.GetProperty("ProductQty").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[8].Name = tOD.GetProperty("TotalPrice").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[9].Name = tOD.GetProperty("Create_Date").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            int row = 2;
            

            
            foreach(OrderDetail item in orderDetails)
            {
                col = 1;
                sheet.Cells[row, col++].Value = orders.OrderID;
                sheet.Cells[row, col++].Value = orders.Receiver;
                sheet.Cells[row, col++].Value = orders.Email;
                sheet.Cells[row, col++].Value = orders.Address;
                sheet.Cells[row, col++].Value = item.ProductID;
                sheet.Cells[row, col++].Value = item.ProductName;
                sheet.Cells[row, col++].Value = item.UserID;
                sheet.Cells[row, col++].Value = item.ProductQty;
                sheet.Cells[row, col++].Value = item.TotalPrice;
                sheet.Cells[row, col++].Value = item.Create_Date.ToString();
                row++;
            }
            MemoryStream ms = new MemoryStream();
            ep.SaveAs(ms);
            ep.Dispose();
            ms.Position = 0;
            string filename = OrderID + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
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
            string rng = "D" + (products.Count+1);  //excel col
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("FirstSheet");
            ExcelTableCollection tblcollection = sheet.Tables;
            ExcelTable table= tblcollection.Add(sheet.Cells["A1:"+ rng],"Product");

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

        public ActionResult MemberList()
        {
            var member = db.Member.ToList();
            return View("MemberList", "_LayoutAdmin", member);
        }

        public ActionResult AddCar(string ProductID)
        {
            string UserID = (Session["Member"] as Member).UserID;

            var currentCar = db.OrderDetail.Where(m=>m.ProductID==ProductID&&m.Approved_Flag==false&&m.UserID==UserID).FirstOrDefault();
            if (currentCar == null)
            {
                var product = db.Product.Where(m => m.ProductID == ProductID).FirstOrDefault();
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.UserID = UserID;
                orderDetail.ProductID = product.ProductID;
                orderDetail.ProductName = product.ProductName;
                orderDetail.TotalPrice = product.ProductPrice;
                orderDetail.ProductQty = 1;
                orderDetail.Approved_Flag = false;
                orderDetail.Create_Date= DateTime.Now;
                
                db.OrderDetail.Add(orderDetail);
            }
            else
            {
                currentCar.ProductQty += 1;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction("CheckCar");
        }

        public void Add_car(string ProductID)
        {
            string UserID = (Session["Member"] as Member).UserID;

            var currentCar = db.OrderDetail.Where(m => m.ProductID == ProductID && m.Approved_Flag == false && m.UserID == UserID).FirstOrDefault();
            if (currentCar == null)
            {
                var product = db.Product.Where(m => m.ProductID == ProductID).FirstOrDefault();
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.UserID = UserID;
                orderDetail.ProductID = product.ProductID;
                orderDetail.ProductName = product.ProductName;
                orderDetail.TotalPrice = product.ProductPrice;
                orderDetail.ProductQty = 1;
                orderDetail.Approved_Flag = false;
                orderDetail.Create_Date = DateTime.Now;

                db.OrderDetail.Add(orderDetail);
            }
            else
            {
                currentCar.ProductQty += 1;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DeleteCar(int OrderDetailID)
        {
            var orderDetail = db.OrderDetail.Where(m => m.OrderDetailID == OrderDetailID).FirstOrDefault();
            db.OrderDetail.Remove(orderDetail);
            db.SaveChanges();
            return RedirectToAction("CheckCar");
        }

        public ActionResult DeleteOrder(string OrderID)
        {
            var order = db.OrderHeader.Where(m => m.OrderID == OrderID).FirstOrDefault();
            var orderDetail = db.OrderDetail.Where(m => m.OrderID == OrderID).ToList();
            foreach(var item in orderDetail)
            {
                item.Delete_Date = DateTime.Now;
                item.Delete_Flag = true;
            }
            order.Delete_Date = DateTime.Now;
            order.Delete_Flag = true;
            db.SaveChanges();
            TempData["DeleteOrderMessage"] = "刪除成功";
            return RedirectToAction("OrderList");
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