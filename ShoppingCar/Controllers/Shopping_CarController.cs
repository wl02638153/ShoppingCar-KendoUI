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
    public class Shopping_CarController : Controller
    {
        //dbShoppingCarEntities3 db = new dbShoppingCarEntities3();     //存取db localdb
        ShoppingCartEntities db = new ShoppingCartEntities();           //.77db
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        // GET: Shopping_Car
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //[Filters.MemberFilter]
        public ActionResult ImportOrder(HttpPostedFileBase ImportOrder)
        {
            string UserID = Session["Member"].ToString();
            FileUploadValidate fs = new FileUploadValidate();
            fs.filesize = 2000;
            string message = "";
            ExcelValidate ev = new ExcelValidate();
            if (fs.UploadUserFile(ImportOrder))
            {
                if (ev.CheckExcelData(ImportOrder))
                {
                    var currentWorkSheet = ev.workbook.Worksheets.First();

                    int col = 1;
                    int row = 2;
                    foreach (var item in currentWorkSheet.Cells)
                    {
                        if (currentWorkSheet.Cells[row, col].Value != null)
                        {
                            try
                            {
                                string ProductID = currentWorkSheet.Cells[row, col].Value.ToString();
                                var currentCar = db.ShoppingCarList.Where(m => m.ProductID == ProductID && m.UserID == UserID).FirstOrDefault();
                                if (currentCar == null)
                                {
                                    var product = db.Product.Where(m => m.ProductID == ProductID).FirstOrDefault();
                                    if (product != null)
                                    {
                                        ShoppingCarList shoppingCarList = new ShoppingCarList();
                                        shoppingCarList.UserID = UserID;
                                        shoppingCarList.ProductID = product.ProductID;
                                        shoppingCarList.ProductQty = 1;
                                        shoppingCarList.Order_Flag = true;
                                        shoppingCarList.Create_Date = DateTime.Now;
                                        db.ShoppingCarList.Add(shoppingCarList);
                                        db.SaveChanges();
                                        message += "<p>[" + row +","+ ProductID + "]新增成功</p>";
                                    }
                                    else
                                    {
                                        message += "<p>[" + row + "]該產品不存在</p>";
                                    }
                                }
                                else
                                {
                                    currentCar.ProductQty += 1;
                                    message += "<p>[" + row + "," + ProductID + "]新增成功</p>";
                                }
                            }
                            catch (DbEntityValidationException ex)
                            {
                                TempData["ImportCarMessage"] = "請確認資料格式是否正確";
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
                                message+=ex.Message;
                            }
                            row++;
                            TempData["ExcelResultErrorMessage"] = message;
                        }
                        else
                        {
                            break;
                        }
                    }
                }  
                return RedirectToAction("Check_Car");
            }
            else
            {
                //ViewBag.ExcelResultErrorMessage = fs.ErrorMessage;
                TempData["ExcelResultErrorMessage"] = fs.ErrorMessage;
                return RedirectToAction("Index","Home");
            }
        }

        //[Filters.MemberFilter]
        public ActionResult ShoppingCar()
        {
            string UserID = Session["Member"].ToString();
            var shoppingCarList = db.ShoppingCarList.Where(m => m.UserID == UserID && m.Order_Flag == true).ToList();
            return View("ShoppingCar", Session["UserTag"].ToString(), shoppingCarList);
        }
        [HttpPost]
        public ActionResult ShoppingCar(string Receiver, string Email, string Address,decimal TotalPrice)    //產生orderHeader
        {
            string UserID = Session["Member"].ToString();
            string guid = Guid.NewGuid().ToString();        //產生唯一的guid變數 for OrderID

            OrderHeader orderHeader = new OrderHeader();
            orderHeader.OrderID = guid;
            orderHeader.UserID = UserID;
            orderHeader.Receiver = Receiver;
            orderHeader.Email = Email;
            orderHeader.Address = Address;
            orderHeader.Price = TotalPrice;
            orderHeader.Create_Date = DateTime.Now;
            db.OrderHeader.Add(orderHeader);
            var carList = db.ShoppingCarList.Where(m => m.Order_Flag == true && m.UserID == UserID).ToList();

            foreach (var item in carList)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.UserID = UserID;
                orderDetail.ProductQty = item.ProductQty;
                orderDetail.ProductID = item.ProductID;
                orderDetail.OrderID = guid;
                orderDetail.Create_Date= DateTime.Now;
                db.OrderDetail.Add(orderDetail);
                db.ShoppingCarList.Remove(item);
                db.SaveChanges();
            }
            TempData["CreateOrderMessage"] = guid + "訂單訂購成功!";
            return RedirectToAction("OrderList");
        }

        public ActionResult Check_Car()
        {
            string UserID = Session["Member"].ToString();
            ShoppingCarCheckList shoppingCarCheckList = new ShoppingCarCheckList();
            shoppingCarCheckList.ShoppingCarLists = db.ShoppingCarList.Where(m => m.UserID == UserID).ToList<ShoppingCarList>();
            return View("Check_Car", Session["UserTag"].ToString(), shoppingCarCheckList);
        }

        [HttpPost]
        public ActionResult Check_Car(ShoppingCarCheckList list,decimal TotalPrice)
        {
            string UserID = Session["Member"].ToString();

            int count = 0;
            foreach(var item in list.ShoppingCarLists)
            {
                if (item.Order_Flag == false) count++;
            }
            if (count == list.ShoppingCarLists.Count)
            {
                TempData["CheckMessage"] = "請選擇商品!";
                return RedirectToAction("Check_Car");
            }
            
            foreach (var item in list.ShoppingCarLists)
            {
                var shoppingCarList = db.ShoppingCarList.Where(m => m.UserID == UserID && m.Id == item.Id).FirstOrDefault();
                shoppingCarList.Order_Flag = item.Order_Flag;
                shoppingCarList.ProductQty = item.ProductQty;
            }
            db.SaveChanges();
            Session["TotalPrice"] = TotalPrice;
            return RedirectToAction("ShoppingCar");
        }

        public ActionResult OrderList(int page = 1)
        {
            int pageSize = 5;
            int currentPage = page < 1 ? 1 : page;
            string UserID = Session["Member"].ToString();
            var orders = db.OrderHeader.Where(m => m.UserID == UserID && m.Delete_Flag != true).OrderByDescending(m => m.Create_Date).ToList();  //取出order依照create date排序
            ViewBag.PageOfOrder= orders.ToPagedList(currentPage, pageSize);
            return View("OrderList", Session["UserTag"].ToString(), ViewBag.PageOfOrder);
        }

        public ActionResult OrderListDetail(string OrderID)
        {
            var orderDetails = db.OrderDetail.Where(m => m.OrderID == OrderID && m.Delete_Flag != true).ToList();
            return View("OrderListDetail", Session["UserTag"].ToString(), orderDetails);
        }

        public ActionResult DownloadOrderExcel(string OrderID)
        {
            string UserID = Session["Member"].ToString();
            var orders = db.OrderHeader.Where(m => m.UserID == UserID && m.OrderID == OrderID).FirstOrDefault();
            var orderDetails = db.OrderDetail.Where(m => m.OrderID == OrderID && m.Delete_Flag != true).ToList();

            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("FirstSheet");
            string rng = "J" + (orderDetails.Count + 1 + 1);  //excel col    //array+1  orders+1
            ExcelTableCollection tblcollection = sheet.Tables;
            ExcelTable table = tblcollection.Add(sheet.Cells["A1:" + rng], "Order");

            var tOH = typeof(OrderHeaderMetaData);
            var tOD = typeof(OrderDetailMetaData);
            var tPD = typeof(ProductMetaData);

            int col = 1;    //欄:直的，因為要從第1欄開始，所以初始為1
            table.Columns[0].Name = tOH.GetProperty("OrderID").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[1].Name = tOH.GetProperty("Receiver").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[2].Name = tOH.GetProperty("Email").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[3].Name = tOH.GetProperty("Address").GetCustomAttribute<DisplayNameAttribute>().DisplayName;

            table.Columns[4].Name = tOD.GetProperty("ProductID").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[5].Name = tPD.GetProperty("ProductName").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[6].Name = tOD.GetProperty("UserID").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[7].Name = tOD.GetProperty("ProductQty").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[8].Name = tPD.GetProperty("ProductPrice").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            int row = 2;



            foreach (OrderDetail item in orderDetails)
            {
                col = 1;
                sheet.Cells[row, col++].Value = orders.OrderID;
                sheet.Cells[row, col++].Value = orders.Receiver;
                sheet.Cells[row, col++].Value = orders.Email;
                sheet.Cells[row, col++].Value = orders.Address;
                sheet.Cells[row, col++].Value = item.ProductID;
                sheet.Cells[row, col++].Value = item.Product.ProductName;
                sheet.Cells[row, col++].Value = item.UserID;
                sheet.Cells[row, col++].Value = item.ProductQty;
                sheet.Cells[row, col++].Value = item.Product.ProductPrice;
                row++;
            }
            MemoryStream ms = new MemoryStream();
            ep.SaveAs(ms);
            ep.Dispose();
            ms.Position = 0;
            string filename = OrderID + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        public ActionResult Download_ALL_Order()
        {
            string UserID = Session["Member"].ToString();

            var q = (from od in db.OrderDetail
                     join os in db.OrderHeader on od.OrderID equals os.OrderID
                     where od.OrderID==os.OrderID && od.UserID.Equals(UserID) &&od.Delete_Flag!=true
                     select new
                     {
                         OrderId = os.OrderID,
                         Receiver = os.Receiver,
                         Email = os.Email,
                         Address = os.Address,
                         ProductID = od.ProductID,
                         ProductName = od.Product.ProductName,
                         UserID = od.UserID,
                         ProductQty = od.ProductQty,
                         Price = os.Price,
                         Create_Date = od.Create_Date
                     }).ToList();

            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("FirstSheet");
            //string rng = "J" + (orderDetails.Count + 1 + 1);  //excel col    //array+1  orders+1
            string rng = "J" + (q.Count + 1 + 1);  //excel col    //array+1  orders+1
            ExcelTableCollection tblcollection = sheet.Tables;
            ExcelTable table = tblcollection.Add(sheet.Cells["A1:" + rng], "Order");

            var tOH = typeof(OrderHeaderMetaData);
            var tOD = typeof(OrderDetailMetaData);
            var tPD = typeof(ProductMetaData);

            int col = 1;    //欄:直的，因為要從第1欄開始，所以初始為1
            table.Columns[0].Name = tOH.GetProperty("OrderID").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[1].Name = tOH.GetProperty("Receiver").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[2].Name = tOH.GetProperty("Email").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[3].Name = tOH.GetProperty("Address").GetCustomAttribute<DisplayNameAttribute>().DisplayName;

            table.Columns[4].Name = tOD.GetProperty("ProductID").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[5].Name = tPD.GetProperty("ProductName").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[6].Name = tOD.GetProperty("UserID").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[7].Name = tOD.GetProperty("ProductQty").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[8].Name = tOH.GetProperty("Price").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            table.Columns[9].Name = tOD.GetProperty("Create_Date").GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            int row = 2;



            foreach (var item in q)
            {
                col = 1;
                sheet.Cells[row, col++].Value = item.OrderId;
                sheet.Cells[row, col++].Value = item.Receiver;
                sheet.Cells[row, col++].Value = item.Email;
                sheet.Cells[row, col++].Value = item.Address;
                sheet.Cells[row, col++].Value = item.ProductID;
                sheet.Cells[row, col++].Value = item.ProductName;
                sheet.Cells[row, col++].Value = item.UserID;
                sheet.Cells[row, col++].Value = item.ProductQty;
                sheet.Cells[row, col++].Value = item.Price;
                sheet.Cells[row, col++].Value = item.Create_Date.ToString();
                row++;
            }
            MemoryStream ms = new MemoryStream();
            ep.SaveAs(ms);
            ep.Dispose();
            ms.Position = 0;
            string filename = UserID + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        public ActionResult AddCar(string ProductID)
        {
            string UserID = Session["Member"].ToString();
            string message = "";
            var currentCar = db.ShoppingCarList.Where(m => m.ProductID == ProductID && m.UserID == UserID).FirstOrDefault();
            if (currentCar == null)
            {
                var product = db.Product.Where(m => m.ProductID == ProductID).FirstOrDefault();
                ShoppingCarList shoppingCarList = new ShoppingCarList();
                shoppingCarList.UserID = UserID;
                shoppingCarList.ProductID = product.ProductID;
                shoppingCarList.ProductQty = 1;
                shoppingCarList.Order_Flag = true;
                shoppingCarList.Create_Date = DateTime.Now;

                db.ShoppingCarList.Add(shoppingCarList);
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

            return RedirectToAction("Check_Car");
        }
        
        public void Add_car(string ProductID)
        {
            string UserID = Session["Member"].ToString();

            var currentCar = db.ShoppingCarList.Where(m => m.ProductID == ProductID && m.UserID == UserID).FirstOrDefault();
            if (currentCar == null)
            {
                var product = db.Product.Where(m => m.ProductID == ProductID).FirstOrDefault();
                if (product != null)
                {
                    ShoppingCarList shoppingCarList = new ShoppingCarList();
                    shoppingCarList.UserID = UserID;
                    shoppingCarList.ProductID = product.ProductID;
                    shoppingCarList.ProductQty = 1;
                    shoppingCarList.Order_Flag = true;
                    shoppingCarList.Create_Date = DateTime.Now;
                    db.ShoppingCarList.Add(shoppingCarList);
                }
                else
                {
                    
                }
                
            }
            else
            {
                currentCar.ProductQty += 1;
            }
        }

        public ActionResult Delete_Car(int Id)
        {
            var shoppingCarId = db.ShoppingCarList.Where(m => m.Id == Id).FirstOrDefault();
            db.ShoppingCarList.Remove(shoppingCarId);
            db.SaveChanges();
            TempData["DeleteCar"] = "刪除成功!";
            return RedirectToAction("Check_Car");
        }

        public ActionResult DeleteCar(int OrderDetailID)
        {
            var orderDetail = db.OrderDetail.Where(m => m.OrderDetailID == OrderDetailID).FirstOrDefault();
            db.OrderDetail.Remove(orderDetail);
            db.SaveChanges();
            TempData["DeleteCar"] = "刪除成功!";
            return RedirectToAction("CheckCar");
        }

        public ActionResult DeleteOrder(string OrderID)
        {
            var order = db.OrderHeader.Where(m => m.OrderID == OrderID).FirstOrDefault();
            var orderDetail = db.OrderDetail.Where(m => m.OrderID == OrderID).ToList();
            foreach (var item in orderDetail)
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
    }
}