using ShoppingCar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string UserID, string Password)
        {
            try
            {
                var member = db.Member.Where(m => m.UserID == UserID && m.Password == Password).FirstOrDefault(); //取得會員並指定給member

                if (member == null)
                {
                    ViewBag.Message = "帳號密碼錯誤，登入失敗，請重新輸入";
                    return View();
                }
                Session["Welcome"] = member.MemberName + "歡迎光臨";
                Session["Member"] = member.MemberName;
                TempData["LoginMessage"] = "登入成功";
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
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
            if (member == null)
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

        public ActionResult MemberList()
        {
            var member = db.Member.ToList();
            return View("MemberList", Session["UserTag"].ToString(), member);
        }
    }
}