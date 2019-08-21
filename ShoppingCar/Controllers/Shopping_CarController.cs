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
                            string ProductID = currentWorkSheet.Cells[row, col].Value.ToString();

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

        //[Filters.MemberFilter]
        public ActionResult ShoppingCar()
        {
            string UserID = Session["Member"].ToString();
            var orderDetails = db.OrderDetail.Where(m => m.UserID == UserID && m.Approved_Flag == true&&m.OrderID==null).ToList();//
            return View("ShoppingCar", Session["UserTag"].ToString(), orderDetails);
        }
        [HttpPost]
        public ActionResult ShoppingCar(string Receiver, string Email, string Address)    //產生orderHeader
        {
            string UserID = Session["Member"].ToString();
            string guid = Guid.NewGuid().ToString();        //產生唯一的guid變數 for OrderID

            OrderHeader orderHeader = new OrderHeader();
            orderHeader.OrderID = guid;
            orderHeader.UserID = UserID;
            orderHeader.Receiver = Receiver;
            orderHeader.Email = Email;
            orderHeader.Address = Address;
            orderHeader.Create_Date = DateTime.Now;
            db.OrderHeader.Add(orderHeader);
            var carList = db.OrderDetail.Where(m => m.Approved_Flag == true && m.UserID == UserID&&m.OrderID==null).ToList();

            foreach (var item in carList)
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
            string UserID = Session["Member"].ToString();
            OrderDetailList detailList = new OrderDetailList();
            detailList.OrderDetails = db.OrderDetail.Where(m => m.UserID == UserID && m.OrderID == null).ToList<OrderDetail>();
            return View("CheckCar", Session["UserTag"].ToString(), detailList);
        }
        [HttpPost]
        public ActionResult CheckCar(OrderDetailList list)
        {
            string UserID = Session["Member"].ToString();
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
                else if (item.Approved_Flag == false)
                {
                    check.Approved_Flag = false;
                }

            }
            db.SaveChanges();

            var orderDetailsNew = db.OrderDetail.Where(m => m.UserID == UserID && m.Approved_Flag == true && m.Delete_Flag != true && m.OrderID == null).ToList();
            //return View("ShoppingCar", "_LayoutMember", orderDetailsNew);
            return RedirectToAction("ShoppingCar", "Shopping_Car", orderDetailsNew);
        }

        public ActionResult OrderList()
        {
            string UserID = Session["Member"].ToString();
            var orders = db.OrderHeader.Where(m => m.UserID == UserID && m.Delete_Flag != true).OrderByDescending(m => m.Create_Date).ToList();  //取出order依照create date排序
            return View("OrderList", Session["UserTag"].ToString(), orders);
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



            foreach (OrderDetail item in orderDetails)
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

        public ActionResult Download_ALL_Order()
        {
            string UserID = Session["Member"].ToString();
            //var orders = db.OrderHeader.Where(m => m.UserID == UserID).ToList();
            //var orderDetails = db.OrderDetail.Where(m => m.OrderID !=null && m.Delete_Flag != true&&m.UserID==UserID).ToList();
            //var q = from od in orderDetails
            //        join os in orders on od.OrderID equals os.OrderID
            //        select new
            //        {
            //            OrderId = os.OrderID,
            //            Receiver = os.Receiver,
            //            Email = os.Email,
            //            Address = os.Address,
            //            ProductID = od.ProductID,
            //            ProductName = od.ProductName,
            //            UserID = od.UserID,
            //            ProductQty = od.ProductQty,
            //            TotalPrice = od.TotalPrice,
            //            Create_Date = od.Create_Date
            //        };


            var q = (from od in db.OrderDetail
                     join os in db.OrderHeader on od.OrderID equals os.OrderID
                     where od.OrderID==os.OrderID && od.UserID.Equals(UserID)  
                     select new
                     {
                         OrderId = os.OrderID,
                         Receiver = os.Receiver,
                         Email = os.Email,
                         Address = os.Address,
                         ProductID = od.ProductID,
                         ProductName = od.ProductName,
                         UserID = od.UserID,
                         ProductQty = od.ProductQty,
                         TotalPrice = od.TotalPrice,
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
                sheet.Cells[row, col++].Value = item.TotalPrice;
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

            return RedirectToAction("CheckCar");
        }

        public void Add_car(string ProductID)
        {
            string UserID = Session["Member"].ToString();

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