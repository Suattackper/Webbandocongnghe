using electronics_shop.Common;
using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace electronics_shop.Controllers
{
    public class AccountController : Controller
    {
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        private Account _currentUser;

        //Check if any of the Email matches the Email specified in the Parameter
        //using the ANY extension method.
        public JsonResult EmailExists(string Email)
        {
            return Json(!db.Accounts.Any(m => m.Email == Email), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PhoneNumberExists(string PhoneNumber)
        {
            return Json(!db.Accounts.Any(m => m.PhoneNumber == PhoneNumber), JsonRequestBehavior.AllowGet);
        }

        // View Register
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
        
        //Registration Process
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

        // View Login
        public ActionResult Login()
        {
            _currentUser = (Account)Session["CurrentUser"];
            if (_currentUser != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModels model)
        {
            // Get current user information from session
            _currentUser = (Account)Session["CurrentUser"];

            //Check if the user is logged in
            if (_currentUser != null)
            {
                return RedirectToAction("Index", "Home");
            }

            if(ModelState.IsValid)
            {
                // Check database to find a user that matches this email and password
                _currentUser = db.Accounts.FirstOrDefault(a => a.Email == model.Email && a.Password == model.Password);
                if(_currentUser != null)
                {
                    Session.Add("CurrentUser", _currentUser);
                    Notification.setNotification1_5s("Login Successfully", "success");
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Email or Password is wrong");
            }
            return View();
        }
    }
}