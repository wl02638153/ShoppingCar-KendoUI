using ShoppingCar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.Security;

namespace ShoppingCar.Controllers
{
    public class MemberController : Controller
    {
        //dbShoppingCarEntities3 db = new dbShoppingCarEntities3();     //存取db
        ShoppingCartEntities db = new ShoppingCartEntities();
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        // GET: Member
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login(System.Uri returnUrl)
        {
            if (returnUrl != null)
            {
                ViewBag.returnUrl = returnUrl.ToString();
                return View();
            }
            return View();
        }

        //[HttpPost]
        //public ActionResult Login(string UserID, string Password/*,string returnUrl*/)
        //{
        //    var member = db.Member.Where(m => m.UserID == UserID && m.Password == Password).FirstOrDefault(); //取得會員並指定給member

        //    if (member == null)
        //    {
        //        ViewBag.Message = "帳號密碼錯誤，登入失敗，請重新輸入";
        //        return View();
        //    }
        //    Session["Welcome"] = member.MemberName + "歡迎光臨";
        //    Session["Member"] = member.MemberName;
        //    TempData["LoginMessage"] = "登入成功";
        //    //return Redirect(returnUrl);
        //    return RedirectToAction("Index","Home");

        //}
        [HttpPost]
        public JsonResult Login(string UserID, string Password/*,string returnUrl*/)
        {
            var member = db.Member.Where(m => m.UserID == UserID && m.Password == Password).FirstOrDefault(); //取得會員並指定給member
            var LoginMessage = "";
            bool isSuccess = false;
            if (member == null)
            {
                LoginMessage = "帳號密碼錯誤，登入失敗，請重新輸入";
                isSuccess = false;
            }
            else
            {
                LoginMessage = "登入成功";
                isSuccess = true;

                var ticket = new FormsAuthenticationTicket(
                    version: 1,
                    name: UserID, //這邊看個人，你想放使用者名稱也可以，自行更改
                    issueDate: DateTime.Now,//現在時間
                    expiration: DateTime.Now.AddMinutes(30),//Cookie有效時間=現在時間往後+30分鐘
                    isPersistent: false,//記住我 true or false
                    userData: UserID, //這邊可以放使用者名稱，而我這邊是放使用者的群組代號
                    cookiePath: FormsAuthentication.FormsCookiePath);
                var encryptedTicket = FormsAuthentication.Encrypt(ticket); //把驗證的表單加密
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);
            }
            var returnData = new
            {
                IsSuccess = isSuccess,
                LoginMessage = LoginMessage,
            };
            Session["Welcome"] = member.MemberName + "歡迎光臨";
            Session["Member"] = member.MemberName;


            return Json(returnData, JsonRequestBehavior.AllowGet);

        }
        //[HttpPost]
        //[ActionName("LoginR")]  //多載
        //public ActionResult Login(string UserID, string Password,string returnUrl)
        //{
        //    var member = db.Member.Where(m => m.UserID == UserID && m.Password == Password).FirstOrDefault(); //取得會員並指定給member

        //    if (member == null)
        //    {
        //        ViewBag.Message = "帳號密碼錯誤，登入失敗，請重新輸入";
        //        return View();
        //    }
        //    Session["Welcome"] = member.MemberName + "歡迎光臨";
        //    Session["Member"] = member.MemberName;
        //    TempData["LoginMessage"] = "登入成功";
        //    return Redirect(returnUrl);
        //}

        [HttpPost]
        [ActionName("LoginR")]  //多載
        public JsonResult Login(string UserID, string Password, string returnUrl)
        {
            var member = db.Member.Where(m => m.UserID == UserID && m.Password == Password).FirstOrDefault(); //取得會員並指定給member
            var LoginMessage="";
            bool isSuccess = false;
            if (member == null)
            {
                LoginMessage = "帳號密碼錯誤，登入失敗，請重新輸入";
                isSuccess = false;
            }
            else
            {
                LoginMessage = "登入成功";
                isSuccess = true;

                var ticket = new FormsAuthenticationTicket(
                   version: 1,
                   name: UserID, //這邊看個人，你想放使用者名稱也可以，自行更改
                   issueDate: DateTime.Now,//現在時間
                   expiration: DateTime.Now.AddMinutes(30),//Cookie有效時間=現在時間往後+30分鐘
                   isPersistent: false,//記住我 true or false
                   userData: UserID, //這邊可以放使用者名稱，而我這邊是放使用者的群組代號
                   cookiePath: FormsAuthentication.FormsCookiePath);
                var encryptedTicket = FormsAuthentication.Encrypt(ticket); //把驗證的表單加密
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);
            }
            var returnData = new
            {
                IsSuccess = isSuccess,
                LoginMessage = LoginMessage,
                ReturnUrl= returnUrl
            };
            Session["Welcome"] = member.MemberName + "歡迎光臨";
            Session["Member"] = member.MemberName;
            return Json(returnData, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Logout()
        {
            Session.Clear();

            TempData["LogoutMessage"] = "成功登出";

            Response.Cookies.Remove(FormsAuthentication.FormsCookieName);

            return RedirectToAction("Index", "Home");
        }

        //public JsonResult Logout()
        //{
        //    Session.Clear();

        //    TempData["LogoutMessage"] = "成功登出";
        //    var LoginMessage="";
        //    var returnData = new
        //    {
        //        LoginMessage = "成功登出"
        //    };

        //    Response.Cookies.Remove(FormsAuthentication.FormsCookieName);

        //    return Json(returnData, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Register(Member rMember)
        {
            db.Configuration.ProxyCreationEnabled = false;
            bool isSuccess = false;
            string RegMessage = "";
            if (ModelState.IsValid)    //model 驗證
            {
                var member = db.Member.Where(m => m.UserID == rMember.UserID).FirstOrDefault();     //if member ==null 代表未註冊
                if (member == null)
                {
                    rMember.Create_Date = DateTime.Now;
                    rMember.Delete_Flag = false;
                    db.Member.Add(rMember);
                    db.SaveChanges();
                    isSuccess = true;
                    RegMessage = "註冊成功!";
                }
                else
                {
                    isSuccess = false;
                    RegMessage = "此帳號已存在";
                }
            }
            else
            {
                isSuccess = false;
                RegMessage = "輸入格式錯誤";
            }
            var returnData = new
            {
                IsSuccess = isSuccess,
                RegMessage = RegMessage,
                ModelStateErrors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray())
            };
            return Json(returnData, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult Register(Member rMember)
        //{
        //    if (ModelState.IsValid == false)    //model 驗證
        //    {
        //        return View();
        //    }
        //    var member = db.Member.Where(m => m.UserID == rMember.UserID).FirstOrDefault();     //if member ==null 代表未註冊
        //    if (member == null)
        //    {
        //        rMember.Create_Date = DateTime.Now;
        //        rMember.Delete_Flag = false;
        //        db.Member.Add(rMember);
        //        db.SaveChanges();
        //        return RedirectToAction("Login");
        //    }
        //    ViewBag.RegisterMessage = "此帳號已有人使用，註冊失敗";
        //    return View();
        //}

        [Filters.MemberFilter]
        public ActionResult MemberList(int page=1)
        {
            int pageSize = 10;
            int currentPage = page < 1 ? 1 : page;
            var member = db.Member.OrderByDescending(m => m.Create_Date).ToList();
            var members = member.ToPagedList(currentPage, pageSize);
            
            return View("MemberList", members);
        }
    }
}