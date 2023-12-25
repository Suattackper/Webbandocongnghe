using CaptchaMvc.HtmlHelpers;
using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Controllers
{
    public class AccountController : Controller
    {
        // Kết nối CSDL
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MessageSent()
        {
            return View();
        }
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
                if (ModelState.IsValid)
                {
                    var check = db.Accounts.Where(m => m.Email == account.Email && m.PhoneNumber == account.PhoneNumber).FirstOrDefault();

                    if (check != null)
                    {
                        // Kiểm tra email và sdt có trong CSDL 
                        ModelState.AddModelError("", "There was a problem creating your account. " +
                            "Your username already exists.");
                        return View();
                    }
                    else
                    {
                        account.Password = Encryptor.MD5Hash(account.Password);
                        account.CreateAt = DateTime.Now;
                        //account.Avatar = "/Content/images/comments/profile_1.png";

                        db.Accounts.Add(account);
                        var result = db.SaveChanges();

                        if (result > 0)
                        {
                            TempData["msgSuccess"] = "Create new account successfully!";
                            return RedirectToAction("Login");
                        }
                    }
                }
                return View();

            }
            catch (Exception ex)
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
            }
            else
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

            var check = db.Accounts.SingleOrDefault(m => m.Email == email && m.Password == password);

            if (ModelState.IsValid)
            {
                if (check == null)
                {
                    ModelState.AddModelError("", "There was a problem logging in. Check your username and password or create an account.");
                }
                else
                {
                    if (!this.IsCaptchaValid(""))
                    {
                        ViewBag.Captcha = "Captcha is not valid";
                    }
                    else
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
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
            }
                return RedirectToAction("Index", "Home");
        }

        // Thay đổi mật khẩu. Xử lý view
        [HttpGet]
        public ActionResult ChangePassword(int accountCode)
        {
            Account account = db.Accounts.Find(accountCode);
            return View();
        }

        //Code xử lý
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Account account, FormCollection collection)
        {
            try
            {
                // MK hiện tại
                var currentPassword = collection["currentPassword"];

                //MK mới 
                var newPassword = collection["newPassword"];

                currentPassword = Encryptor.MD5Hash(currentPassword.Trim());
                newPassword = Encryptor.MD5Hash(newPassword.Trim());

                // Kiểm tra có email tương ứng và password (currentpass) trong csdl 
                var check = db.Accounts.Where(m => m.Password == currentPassword && m.AccountCode == account.AccountCode).FirstOrDefault();

                if (check != null) // Có trong CSDL 
                {
                    check.Password = newPassword;
                    db.SaveChanges();

                    TempData["msgChangePassword"] = "Change password successfully";

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect password!");
                    return View();
                }

            }
            catch (Exception ex)
            {
                TempData["msgChangePasswordFailed"] = "Edit failed!" + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }


        //Edit profie
        [HttpGet]
        public ActionResult EditProfile(int accountCode)
        {
            Account account = db.Accounts.Find(accountCode);
            Session["imgPath"] = account.Avatar;
            return View();
        }

        //Code xử lý thông tin cá nhân 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(Account account, HttpPostedFileBase uploadFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (uploadFile != null)
                    {
                        var fileName = Path.GetFileName(uploadFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images/comments/"), fileName);

                        //account.Avatar = "/Content/images/comments/" + fileName;
                        db.Entry(account).State = EntityState.Modified;

                        // Lấy đường dẫn ảnh (absolute path - đường dẫn tuyệt đối)
                        string oldImgPath = Request.MapPath(Session["imgPath"].ToString());

                        // Lấy đường dẫn ảnh (relative path)
                        var avatarName = Session["imgPath"].ToString();

                        // Kiểm tra ảnh có trùng với avatar của member nào không
                        //var checkAvatar = db.Accounts.Where(m => m.Avatar == avatarName).ToList();

                        if (db.SaveChanges() > 0)
                        {
                            uploadFile.SaveAs(path);

                            // Nếu tồn tại hình trong folder và không member nào có hình này thì xóa ra khỏi folder
                            //if (System.IO.File.Exists(oldImgPath) && checkAvatar.Count < 2)
                            {
                                System.IO.File.Delete(oldImgPath);
                            }

                            // Lấy thông tin mới cập nhập lưu vào session
                            var info = db.Accounts.Where(m => m.AccountCode == account.AccountCode).SingleOrDefault();
                            Session["info"] = info;

                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        //account.Avatar = Session["imgPath"].ToString();

                        db.Entry(account).State |= EntityState.Modified;

                        if (db.SaveChanges() > 0)
                        {
                            // Lấy thông tin mới cập nhập lưu vào session
                            var info = db.Accounts.Where(m => m.AccountCode == account.AccountCode).SingleOrDefault();

                            Session["info"] = info;
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["msgEditProfieFailed"] = "Edit failed! " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}