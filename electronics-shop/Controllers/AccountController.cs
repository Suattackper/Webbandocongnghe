using CaptchaMvc.HtmlHelpers;
using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Controllers
{
    public class AccountController : Controller
    {
        // Kết nối CSDL
        private ECOMMERCEEntities db = new ECOMMERCEEntities(); 

        //View đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        //Code xử lý code Đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account account)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var check = db.Account.Where(m => m.Email == account.Email && m.PhoneNumber == account.PhoneNumber).FirstOrDefault();

                    if(check != null)
                    {
                        // Kiểm tra email và sdt có trong CSDL 
                        ModelState.AddModelError("", "There was a problem creating your account. Your username already exists.");
                        return View();
                    } else
                    {
                        account.Password = Encryptor.MD5Hash(account.Password);
                        account.CreateAt = DateTime.Now;
                        account.Avatar = "~Content/images/comment/profile_1.png";

                        db.Account.Add(account);
                        var result = db.SaveChanges ();
                        
                        if(result > 0)
                        {
                            TempData["msgSuccess"] = "Create new account successfully!";
                            return RedirectToAction("Login");
                        }
                    }
                }
                return View();

            } catch (Exception ex)
            {
                TempData["msgSuccess"] = "Create account failed!" + ex.Message;
                return RedirectToAction("Login");
            }
        }


        //View Đăng nhập 
        [HttpGet]
        public ActionResult Login()
        {
            // Nếu đã đăng nhập rồi sẽ điều hướng sang trang chủ
            if (Session["info"] != null)
            {
                return RedirectToAction("Index", "Home");
            } else
            {
                return View();
            }
        }

        //Code xử lý đăng nhập 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection collection)
        {
            var email = collection["email"];
            var password = collection["password"];

            password = Encryptor.MD5Hash(password);

            var check = db.Account.SingleOrDefault(m => m.Email == email && m.Password == password);

            if(ModelState.IsValid)
            {
                if(check == null)
                {
                    ModelState.AddModelError("", "There was a problem logging in. Check your username and password or create an account.");
                } else
                {
                    if(!this.IsCaptchaValid(""))
                    {
                        ViewBag.Captcha = "Captcha is not valid";
                    } else
                    {
                        Session["info"] = check;
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }

        // View Đăng xuất
        public ActionResult Logout()
        {
            Session.Remove("info");
            return RedirectToAction("Index", "Home");
        }

    }
}