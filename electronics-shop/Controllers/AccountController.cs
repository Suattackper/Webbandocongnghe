using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Controllers
{
    public class AccountController : Controller
    {
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        private Account _currentUser;

        public ActionResult Register()
        {
            _currentUser = (Account)Session["CurrentUser"];
            //Check To See If There is already a User Logged In
            if (_currentUser != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModels model)
        {
            _currentUser = (Account)Session["CurrentUser"];
            if (_currentUser != null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }
            string fail = "";
            string success = "";

            var checkMail = db.Accounts.Any(m => m.Email == model.Email);
            var checkPhone = db.Accounts.Any(m => m.PhoneNumber == model.PhoneNumber);
            if (checkMail)
            {
                fail = "<script>alert('This Email is already in use');</script>";
                return View();
            }
            if (checkPhone)
            {
                fail = "<script>alert('This phone number is already in use');</script>";
                return View();
            }

            _currentUser = new Account();
            _currentUser.Email = model.Email;
            _currentUser.FullName = model.FullName;
            _currentUser.PhoneNumber = model.PhoneNumber;
            _currentUser.Password = model.Password;
            _currentUser.ConfirmPassword = model.ConfirmPassword;
            _currentUser.CreateAt = DateTime.Now;
            db.Accounts.Add(_currentUser);
            db.SaveChanges();
            success = "<script>alert('Đăng ký thành công');</script>";
            Session.Add("CurrentUser", _currentUser);
            return RedirectToAction("Index", "Home");
        }

        // View Đăng nhập
        public ActionResult Login()
        {
            return View();
        }
    }
}