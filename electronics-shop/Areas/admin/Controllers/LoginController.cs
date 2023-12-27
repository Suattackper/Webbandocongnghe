using CaptchaMvc.HtmlHelpers;
using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace electronics_shop.Areas.admin.Controllers
{
    public class LoginController : Controller
    {

        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        // GET: admin/LoginAdmin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAdmin(FormCollection collection)
        {
            var email = collection["email"];
            var password = collection["password"];

            password = Encryptor.MD5Hash(password);

            var notMember = db.Accounts.Where(m => m.RoleID == 3).SingleOrDefault(m => m.Email == email && m.Password == password);

            var check = db.Accounts.Where(m => m.RoleID == 1 || m.RoleID == 2).SingleOrDefault(m => m.Email == email && m.Password == password);

            if (ModelState.IsValid)
            {
                if (check == null)
                {
                    if (notMember != null)
                    {
                        ModelState.AddModelError("", "Role is not invalid.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "There was a problem logging in. Check your email and password!");
                    }
                }
                else
                {
                    if (!this.IsCaptchaValid(""))
                    {
                        ViewBag.Captcha = "Captcha is not valid!";
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(check.FirstName, false);

                        Session["EmailAdmin"] = check.Email;
                        Session["InfoAdmin"] = check;

                        return RedirectToAction("Index", "Dashboard");
                    }
                }
            }
            return View();
        }


        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("LoginAdmin");
        }

    }
}