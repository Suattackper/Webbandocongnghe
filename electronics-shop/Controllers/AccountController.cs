using CaptchaMvc.HtmlHelpers;
using electronics_shop.Models;
using System.IO;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using System.Net;

namespace electronics_shop.Controllers
{
    public class AccountController : Controller
    {
        //Kết nối CSDL
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
                if (ModelState.IsValid)
                {
                    var check = db.Account.Where(m => m.Email == account.Email && m.PhoneNumber == account.PhoneNumber).FirstOrDefault();

                    if (check != null)
                    {
                        // Kiểm tra email và sdt có trong CSDL 
                        ModelState.AddModelError("", "There was a problem creating your account. " +
                            "Your email already exists.");
                        return View();
                    }
                    else
                    {
                        string imagePath = Server.MapPath("~/Content/images/comments/profile_1.png");
                        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

                       
                        account.RoleID = 3;
                        account.AccountStatus = true;
                        account.Password = Encryptor.MD5Hash(account.Password);
                        account.CreateAt = DateTime.Now;
                        account.Avatar = imageBytes;

                        db.Account.Add(account);
                        var result = db.SaveChanges();

                        if (result > 0)
                        {
                            TempData["msgSuccess"] = "Create new account successfully!";
                            return RedirectToAction("Login");
                        }
                    }
                }
                return View(account);

            }
            catch (Exception ex)
            {
                TempData["msgSuccess"] = "Create account failed!" + ex.Message;
                Console.WriteLine(ex.Message);
                return RedirectToAction("Register");
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

            var check = db.Account.SingleOrDefault(m => m.Email == email && m.Password == password);

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
            return RedirectToAction("Index", "Home");
        }

        // Thay đổi mật khẩu. Xử lý view
        [HttpGet]
        public ActionResult ChangePassword(int accountCode)
        {
            Account account = db.Account.Find(accountCode);
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
                var check = db.Account.Where(m => m.Password == currentPassword && m.AccountCode == account.AccountCode).FirstOrDefault();

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


        // Huỳnh Như 20/12/23 3:44 PM
        public ActionResult ResetPassword()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        /*===============================*/

        
        //Edit profie
        [HttpGet]
        public ActionResult EditProfile(int accountCode)
        {
            Account account = db.Account.Find(accountCode);
            Session["imgPath"] = account.Avatar;
            return View(account);
        }

        // Huỳnh Như 19/12/23 11:50 PM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile (Account account, HttpPostedFileBase uploadFile)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    if(uploadFile != null)
                    {
                        var fileName = Path.GetFileName(uploadFile.FileName);

                        var path = Path.Combine(Server.MapPath("~/Content/images/comments/"), fileName);

                        // Convert hình ảnh thành byte array và lưu vào cơ sở dữ liệu
                        using (var binaryReader = new BinaryReader(uploadFile.InputStream))
                        {
                            account.Avatar = binaryReader.ReadBytes(uploadFile.ContentLength);
                        }

                        // Cập nhật đường dẫn hình ảnh mới cho thành viên
                        db.Entry(account).State = EntityState.Modified;


                        // Lấy đường dẫn hình ảnh cũ và xóa nếu không còn được sử dụng
                        string oldImgPath = Request.MapPath(Session["imgPath"].ToString()); // Lấy đường dẫn ảnh (absolute path)

                        var avatarName = Session["imgPath"].ToString(); // Lấy đường dẫn ảnh (relative path)

                        // Chuyển đổi sang bytes

                        byte[] imgBytes = System.IO.File.ReadAllBytes(avatarName);

                        // Kiểm tra ảnh có trùng với avatar của member nào không
                        var checkAvatar = db.Account.Where(model => model.Avatar == imgBytes).ToList();


                        // nếu checkavatar là null hoặc không có phần tử nào được tìm thấy trong truy vấn
                        if (checkAvatar == null)
                        {
                            // lưu trữ hình ảnh mới vào cơ sở dữ liệu
                            fileName = Path.GetFileName(uploadFile.FileName);
                            path = Path.Combine(Server.MapPath("~/Content/images/comments/"), fileName);

                            using (var binaryReader = new BinaryReader(uploadFile.InputStream))
                            {
                                account.Avatar = binaryReader.ReadBytes(uploadFile.ContentLength);
                            }
                            
                            db.Entry(account).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                        if (db.SaveChanges() > 0)
                        {
                            uploadFile.SaveAs(path);
                            if (System.IO.File.Exists(oldImgPath) && checkAvatar.Count < 2) // Nếu tồn tại hình trong folder và không member nào có hình này thì xóa ra khỏi folder
                            {
                                System.IO.File.Delete(oldImgPath);
                            }
                            // Lấy thông tin mới cập nhập lưu vào session
                            var info = db.Account.Where(model => model.AccountCode == account.AccountCode).SingleOrDefault();
                            Session["info"] = info;
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        if (Session["imgPath"] != null)
                        {
                            string imagePath = Session["imgPath"].ToString();
                            byte[] imageBytes = System.IO.File.ReadAllBytes(Server.MapPath(imagePath));
                            account.Avatar = imageBytes;
                            // Tiếp tục xử lý logic của bạn
                        }

                        db.Entry(account).State = EntityState.Modified;

                        if (db.SaveChanges() > 0)
                        {
                            // Lấy thông tin mới cập nhập lưu vào session
                            var info = db.Account.Where(model => model.AccountCode == account.AccountCode).SingleOrDefault();
                            Session["info"] = info;
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", account.RoleID);
                return RedirectToAction("EditProfile", "Account", new { accountCode = account.AccountCode });

            } catch (Exception ex)
            {
                TempData["msgEditProfieFailed"] = "Edit failed! " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

    }
}