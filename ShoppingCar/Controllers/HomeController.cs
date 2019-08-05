using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingCar.Models;
using System.IO;

namespace ShoppingCar.Controllers
{
    public class HomeController : Controller
    {
        dbShoppingCarEntities3 db = new dbShoppingCarEntities3();     //存取db
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
                byte[] FileBytes = Convert.FromBase64String(base64str);
                using (MemoryStream ms = new MemoryStream())
                {
                    
                    //ImgFile.InputStream.CopyTo(ms);
                    FileBytes = ms.GetBuffer();
                }
                cProduct.ProductImg_DB = FileBytes;
            }
            return View("CreateProduct", "_LayoutAdmin");
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
            var orderDetails = db.OrderDetail.Where(m => m.UserID==UserID && m.Approved_Flag == true).ToList();
            return View("ShoppingCar","_LayoutMember",orderDetails);
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
            var carList = db.OrderDetail.Where(m => m.Approved_Flag == false && m.UserID == UserID).ToList();

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
            detailList.OrderDetails = db.OrderDetail.Where(m => m.UserID == UserID&&m.OrderID==null).ToList<OrderDetail>();
            return View(detailList);
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
            return View("ShoppingCar", "_LayoutMember", orderDetailsNew);
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

            return RedirectToAction("ShoppingCar");
        }

        public ActionResult DeleteCar(int OrderDetailID)
        {
            var orderDetail = db.OrderDetail.Where(m => m.OrderDetailID == OrderDetailID).FirstOrDefault();
            db.OrderDetail.Remove(orderDetail);
            db.SaveChanges();
            return RedirectToAction("ShoppingCar");
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