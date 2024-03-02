using CaptchaMvc.HtmlHelpers;
using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices.CompensatingResourceManager;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using electronics_shop.Common;
using System.Drawing.Drawing2D;
using System.Web.Services.Description;
using System.Text;
using System.Security.Principal;
using PagedList;

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
                    var checkMail = db.Accounts.Where(m => m.Email == account.Email).FirstOrDefault();
                    var checkPhone = db.Accounts.Where(m => m.PhoneNumber == account.PhoneNumber).FirstOrDefault();

                    if (checkMail != null && checkPhone != null)
                    {
                        // Kiểm tra email và sdt có trong CSDL 
                        ModelState.AddModelError("", "There was a problem creating your account. " +
                            "Your email or phone number have already existed!");
                        return View();
                    }
                    else
                    {

                        string imagePath = Server.MapPath("~/Content/images/comments/profile_1.png");
                        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

                        account.RoleID = 3;
                        //account.AccountStatus = true;
                        account.Password = Encryptor.MD5Hash(account.Password);
                        account.CreateAt = DateTime.Now;
                        account.Avatar = imageBytes;

                        account.Update_By = account.Email;
                        account.Update_At = DateTime.Now;

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
                    ModelState.AddModelError("", "There was a problem logging in. Check your password or email or create an account.");
                }
                else
                {
                    if (!this.IsCaptchaValid(""))
                    {
                        ViewBag.Captcha = "Captcha is not valid";
                    }
                    else
                    {
                        Session["UserId"] = check.AccountCode;
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
                    check.Update_By = check.Email;
                    check.Update_At = DateTime.Now;
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
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            Account account = db.Accounts.Find(accountCode);
            Session["imgPath"] = account.Avatar;
            return View(account);
        }
        [HttpPost]
        public ActionResult EditProfile(string code, string firstname, string lastname, string phone, string email)
        {
            HttpPostedFileBase image = Request.Files["image"];
            int id = int.Parse(code);
            Account p = db.Accounts.FirstOrDefault(h => h.AccountCode == id);
            List<Account> checkphone = db.Accounts.Where(c => c.PhoneNumber == phone).ToList();
            if((p.PhoneNumber == phone && checkphone.Count > 1) || (p.PhoneNumber != phone && checkphone.Count > 0))
            {
                TempData["Error"] = $"Error -- Số điện thoại {phone} đã tồn tại!";
                return RedirectToAction("EditProfile", new { accountCode = id });
            }
            List<Account> checkemail = db.Accounts.Where(c => c.Email == email).ToList();
            if ((p.Email == email && checkemail.Count > 1) || (p.Email != email && checkphone.Count > 0))
            {
                TempData["Error"] = $"Error -- Email {email} đã tồn tại";
                return RedirectToAction("EditProfile", new { accountCode = id });
            }
            p.FirstName = firstname;
            p.LastName = lastname;
            p.PhoneNumber = phone;
            p.Email = email;
            // Kiểm tra xem có file được chọn không
            if (image != null && image.ContentLength > 0)
            {
                // Đọc dữ liệu từ luồng dữ liệu của file
                byte[] imageData = new byte[image.ContentLength];
                image.InputStream.Read(imageData, 0, image.ContentLength);
                p.Avatar = imageData;
            }
            db.SaveChanges();
            Session["info"] = p;
            return RedirectToAction("EditProfile", new { accountcode = p.AccountCode });
        }
        //Huynh nhu 21/12 2:59 PM
        //Gửi mail
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/Account/ResetPassword" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress(EmailConfig.emailID, EmailConfig.emailName);
            var toEmail = new MailAddress(emailID);

            //có thể thay bằng mật khẩu gmail của bạn
            var fromEmailPassword = EmailConfig.emailPassword;

            //dùng body mail html , file template nằm trong thư mục "EmailTemplate/Text.cshtml"
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "ResetPassword" + ".cshtml");
            string subject = "Update new password";

            //hiển thị nội dung lên form html
            body = body.Replace("{{viewBag.Confirmlink}}", link);

            //hiển thị nội dung lên form html
            body = body.Replace("{{viewBag.Confirmlink}}", Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl));

            var smtp = new SmtpClient
            {
                Host = EmailConfig.emailHost,
                Port = 587,
                //bật ssl
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);

        }


        //[HttpGet]
        //public ActionResult ForgotPassword()
        //{
        //    // Nếu đã đăng nhập rồi sẽ điều hướng sang trang chủ
        //    if (Session["info"] != null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}
        public ActionResult ForgotPassword1()
        {
            if (TempData.ContainsKey("error"))
            {
                ViewBag.e = TempData["error"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword1(string email)
        {
            TempData["error"] = email;
            Account a = db.Accounts.FirstOrDefault(p => p.Email == email);
            if (a != null)
            {
                return RedirectToAction("ForgotPassword2", new { email = email });
            }
            else return View("ErrorForgotPassword");
            //else return RedirectToAction("ForgotPassword1");

        }
        public ActionResult ForgotPassword2( string email)
        {
            ViewBag.Email = email;
            Account a = db.Accounts.FirstOrDefault(p => p.Email == email);
            if(a != null)
            {
                // Tạo đối tượng Random
                Random random = new Random();
                // Tạo số nguyên ngẫu nhiên có 4 chữ số
                int code = random.Next(1000, 10000);
                //Lưu csdl
                a.RequestCode = code.ToString();
                db.SaveChanges();
                //xử lý gửi mail ở đây
                // Thông tin tài khoản email
                string fromEmail = "baitoan88@gmail.com";
                string password = "tkea vcgz hdvj rjmw";
                // Địa chỉ người nhận
                string toEmail = email;
                // Tạo đối tượng MailMessage
                MailMessage mail = new MailMessage(fromEmail, toEmail);
                // Tiêu đề và nội dung email
                mail.Subject = "Besnik. send Code!";
                mail.Body = $"Xin chào, cảm ơn bạn đã sử dụng web Besnik của chúng tôi! \n {code} là code lấy lại mật khẩu của bạn, có hiệu lực trong 15 phút!";
                // Cấu hình đối tượng SmtpClient
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587; // Port thường là 587 cho SMTP over TLS (SSL)
                smtpClient.Credentials = new NetworkCredential(fromEmail, password);
                smtpClient.EnableSsl = true;
                // Gửi email
                smtpClient.Send(mail);
                //ViewBag.success = "Email sent successfully!";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOutForm(OrderViewModel req, string total, string promotion)
        {
            var code = new { Success = false, Code = -1 };


            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                string magiamgia = "";
                int? mucgiam = 0;
                if (promotion != null && promotion != "")
                {
                    //string x = (string)Session["promotioncode"];
                    Promotion p = db.Promotions.FirstOrDefault(j => j.PromotionCode == promotion);
                    magiamgia = p.PromotionCode;
                    mucgiam = p.PromotionPercentage;
                }
                if (cart != null)
                {
                    Order order = new Order();
                    //Product product = new Product();
                    order.AccountAddress = new AccountAddress();

                    if (Session["info"] != null)
                    {

                        int userID = (int)Session["UserID"];
                        var account = db.Accounts.FirstOrDefault(a => a.AccountCode == userID);
                        order.Account = account;

                        order.Account.FirstName = account.FirstName;

                        order.Account.LastName = account.LastName;
                        order.Account.Email = account.Email;
                        order.AccountAddress.PhoneNumber = account.PhoneNumber;
                    }
                    if (magiamgia != "")
                    {
                        order.PromotionCode = magiamgia;
                        Promotion c = db.Promotions.FirstOrDefault(p => p.PromotionCode == magiamgia);
                        c.Quantity--;
                        db.SaveChanges();
                    }
                    order.DeliveryCode = "GHN";

                    order.AccountAddress.Number = req.Number;
                    order.AccountAddress.District = req.District;
                    order.AccountAddress.Ward = req.Ward;
                    order.AccountAddress.City = req.City;

                    order.AccountCode = (int)Session["UserId"]; // chỗ này nhập accountcode đã có trong csdl của m, hoặc truyên fvaof khi đăng nhập thành công
                    order.AccountAddressCode = order.AccountAddressCode;  // chỗ này tạo đại 1 accountaddress trong csdl r nhập accountaddresscode vào
                    order.PaymentCode = req.TypePayment;
                    // chỗ này tạo đại 1 payment trong csdl r nhập PaymentCode vào
                    order.OrderDate = DateTime.Now;
                    order.OrderTotal = decimal.Parse(total); //(decimal?)cart.Items.Sum(x => (x.Price * x.Quantity));   // chỗ này nhập tổng bill vào
                    order.OrderNote = "";  // chỗ này nhập ghi chú vaof 
                    order.Delivered = false; //chưa vận chuyển
                    //product.Quantity = product.Quantity - cart.Items.Sum(x=>(x.Quantity));
                    //db.SaveChanges();
                    db.Orders.Add(order);
                    db.SaveChanges();
                    ViewBag.TypePayment = order.PaymentCode;
                    //lấy mã ordercode
                    Order ordercode = db.Orders.OrderByDescending(o => o.OrderCode).FirstOrDefault();


                    //cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetail
                    //{
                    //    OrderCode = ordercode.OrderCode,
                    //    ProductCode = x.ProductId,
                    //    Quantity = x.Quantity,
                    //    Price = (decimal?)x.Price
                    //}));
                    foreach (var i in cart.Items)
                    {
                        OrderDetail p = new OrderDetail();
                        p.ProductCode = i.ProductId;
                        p.Quantity = i.Quantity;
                        p.Price = decimal.Parse(i.Price.ToString());
                        p.OrderCode = order.OrderCode;
                        p.Total = decimal.Parse((i.Price * i.Quantity).ToString());

                        Product f = db.Products.FirstOrDefault(j => j.ProductCode == i.ProductId);
                        List<Promotion> check = db.Promotions.Where(h => h.StartDate <= DateTime.Now && h.EndDate >= DateTime.Now && h.Quantity > 0).ToList();
                        foreach (var item in check)
                        {
                            if (f.PromotionCode == item.PromotionCode)
                            {
                                Promotion n = db.Promotions.FirstOrDefault(x => x.PromotionCode == item.PromotionCode);
                                n.Quantity--;
                                db.SaveChanges();
                                p.Total = decimal.Parse((p.Total - p.Total * n.PromotionPercentage / 100).ToString());
                                break;
                            }

                        }
                        db.OrderDetails.Add(p);
                        db.SaveChanges();

                    }
                    var listorderdetail = db.OrderDetails.Where(p => p.OrderCode == ordercode.OrderCode).ToList();
                    foreach (var i in listorderdetail)
                    {
                        Product p = db.Products.FirstOrDefault(h => h.ProductCode == i.ProductCode);
                        p.Quantity = p.Quantity - i.Quantity;
                        db.SaveChanges();
                    }

                    //order.OrderTotal = (decimal?)cart.Items.Sum(x => (x.Price * x.Quantity));
                    //order.typepayment = req.typepayment;
                    //order.OrderDate = DateTime.Now;

                    //order.CreateBy = req.PhoneNumber;
                    //Random rd = new Random();
                    //order.OrderCode = rd.Next(0,9) + rd.Next(0, 9) + rd.Next(0, 9);

                    db.SaveChanges();
                    // send mail cho khach hang
                    var strSanPham = "";
                    var thanhtien = decimal.Zero;
                    var TongTien = decimal.Zero;
                    foreach (var sp in cart.Items)
                    {
                        strSanPham += "<tr>";
                        strSanPham += "<td>" + sp.ProductName + "</td>";
                        strSanPham += "<td>" + sp.Quantity + "</td>";
                        strSanPham += "<td>" + string.Format("₫{0:#,0}", sp.ToTalPrice) + "</td>";
                        strSanPham += "</tr>";
                        thanhtien += (decimal)(sp.Price * sp.Quantity);
                    }
                    if (mucgiam != 0)
                    {
                        TongTien = thanhtien - thanhtien * (decimal)mucgiam / 100 + 30000; //+vanchuyen
                    }
                    else TongTien = thanhtien + 30000; //+vanchuyen
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}", order.OrderCode.ToString());
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    if (Session["info"] != null)
                    {
                        int userID = (int)Session["UserID"];
                        var account = db.Accounts.FirstOrDefault(a => a.AccountCode == userID);
                        contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", account.FirstName + " " + account.LastName);
                        contentCustomer = contentCustomer.Replace("{{Phone}}", account.PhoneNumber);
                        contentCustomer = contentCustomer.Replace("{{Email}}", account.Email);


                    }
                    contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.AccountAddress.Number + ", " + order.AccountAddress.Ward + ", " + order.AccountAddress.District + ", " + order.AccountAddress.City);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", order.OrderDate.ToString());
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", string.Format("₫{0:#,0}", thanhtien));
                    contentCustomer = contentCustomer.Replace("{{TongTien}}", string.Format("₫{0:#,0}", TongTien));
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);

                    electronics_shop.Common.Common.SendMail("Beszik.", "Đơn hàng #" + order.OrderCode, contentCustomer.ToString(), req.Email);

                    //string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                    //contentAdmin = contentAdmin.Replace("{{MaDon}}", order.OrderCode.ToString());
                    //contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                    //if (Session["info"] != null)
                    //{
                    //    int userID = (int)Session["UserID"];
                    //    var account = db.Accounts.FirstOrDefault(a => a.AccountCode == userID);
                    //    contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", account.FirstName + " " + account.LastName);
                    //    contentAdmin = contentAdmin.Replace("{{Phone}}", account.PhoneNumber);
                    //    contentAdmin = contentAdmin.Replace("{{Email}}", account.Email);

                    //}
                    //contentAdmin = contentAdmin.Replace("{{SoNha+Phuong}}", order.AccountAddress.Number + " " + order.AccountAddress.Ward);
                    //contentAdmin = contentAdmin.Replace("{{Tinh}}", order.AccountAddress.District);
                    //contentAdmin = contentAdmin.Replace("{{ThanhPho}}", order.AccountAddress.City);
                    //contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.AccountAddress.Number);
                    //contentAdmin = contentAdmin.Replace("{{NgayDat}}", order.OrderDate.ToString());
                    //contentAdmin = contentAdmin.Replace("{{ThanhTien}}", string.Format("₫{0:#,0}", thanhtien));
                    //contentAdmin = contentAdmin.Replace("{{TongTien}}", string.Format("₫{0:#,0}", TongTien));
                    //contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                    //electronics_shop.Common.Common.SendMail("Beszik.", "Đơn hàng mới #" + order.OrderCode, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                    code = new { Success = true, Code = 1 };
                    cart.ClearCart();
                    return RedirectToAction("CheckOutSuccess");
                }
            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult ForgotPassword2(string email, string so1, string so2, string so3, string so4)
        {
            string code = so1 + so2 + so3 + so4;
            Account a = db.Accounts.FirstOrDefault(p => p.Email == email);
            if (a != null)
            {
                if(a.RequestCode == code)
                {
                    // Tạo đối tượng Random
                    Random random = new Random();
                    // Phạm vi ký tự chữ cái
                    const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                    // Phạm vi số
                    const string numbers = "0123456789";
                    // Phạm vi ký tự đặc biệt
                    const string specialCharacters = "!@#$%^&*()_+[]{}|;:'\",.<>?/";
                    // Kết hợp tất cả các ký tự
                    string allCharacters = alphabet + numbers + specialCharacters;
                    // Tạo chuỗi ngẫu nhiên
                    StringBuilder randomString = new StringBuilder();
                    int length = 8; // Độ dài của chuỗi ngẫu nhiên (có thể điều chỉnh)
                    for (int i = 0; i < length; i++)
                    {
                        // Chọn một ký tự ngẫu nhiên từ allCharacters
                        char randomChar = allCharacters[random.Next(allCharacters.Length)];
                        // Thêm ký tự vào chuỗi ngẫu nhiên
                        randomString.Append(randomChar);
                    }
                    // luu csdl
                    a.Password = Encryptor.MD5Hash(randomString.ToString());
                    a.RequestCode = "";
                    db.SaveChanges();
                    //xử lý gửi mail ở đây
                    // Thông tin tài khoản email
                    string fromEmail = "baitoan88@gmail.com";
                    string password = "tkea vcgz hdvj rjmw";
                    // Địa chỉ người nhận
                    string toEmail = email;
                    // Tạo đối tượng MailMessage
                    MailMessage mail = new MailMessage(fromEmail, toEmail);
                    // Tiêu đề và nội dung email
                    mail.Subject = "Besnik. send new password!";
                    mail.Body = $"Xin chào, cảm ơn bạn đã sử dụng web Besnik của chúng tôi! \n {randomString} là mật khẩu mới của bạn!";
                    // Cấu hình đối tượng SmtpClient
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                    smtpClient.Port = 587; // Port thường là 587 cho SMTP over TLS (SSL)
                    smtpClient.Credentials = new NetworkCredential(fromEmail, password);
                    smtpClient.EnableSsl = true;
                    // Gửi email
                    smtpClient.Send(mail);
                    //ViewBag.success = "Email sent successfully!";
                    return RedirectToAction("ForgotPassword3");
                }
            }
            return View("ErrorForgotPassword");
        }
        public ActionResult ForgotPassword3()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(Account account)
        {

            try
            {
                // kiểm tra email đã trùng với email đăng ký tài khoản chưa, nếu chưa đăng ký sẽ trả về fail
                var check = db.Accounts.Where(m => m.Email == account.Email).FirstOrDefault();

                if (check != null)
                {
                    // Gửi email reset password
                    string resetCode = Guid.NewGuid().ToString();


                    // gửi code reset đến mail đã nhập ở form quên mật khẩu , kèm code resetpass,  tên tiêu đề gửi
                    SendVerificationLinkEmail(account.Email, resetCode);

                    string sendmail = account.Email;
                    //request code phải giống reset code  
                    account.RequestCode = resetCode;

                    db.SaveChanges();

                    Notification.setNotification5s("Đường dẫn reset password đã được gửi, vui lòng kiểm tra email", "success");


                }
                else
                {
                    Notification.setNotification1_5s("Email chưa tồn tại trong hệ thống", "error");
                }

                return View(account);
            }
            catch (Exception ex)
            {
                TempData["msgChangePasswordFailed"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // Huỳnh Như 20/12/23 3:44 PM
        [HttpGet]
        public ActionResult ResetPassword(string codeRequest)
        {
            try
            {
                Account code = db.Accounts.Where(m => m.RequestCode == codeRequest).FirstOrDefault();

                if (code != null && !User.Identity.IsAuthenticated)
                {
                    Account model = new Account();
                    model.RequestCode = codeRequest;
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["msgChangePasswordFailed"] = "Edit failed!" + ex.Message;
                return RedirectToAction("Login");
            }
        }

        //Huynh nhu 23/12 2:33 PM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(Account account)
        {
            var check = db.Accounts.Where(m => m.RequestCode == account.RequestCode).FirstOrDefault();

            if (check != null)
            {
                check.Password = Encryptor.MD5Hash(account.Password);

                //Sau khi đổi mật khẩu thành công, khi quay lại link cũ thì sẽ không đôi được mật khẩu nữa 
                check.RequestCode = "";
                check.Update_By = check.Email;
                check.Update_At = DateTime.Now;

                db.SaveChanges();

                Notification.setNotification1_5s("Update new password successfully", "success");

                return RedirectToAction("Login");
            }

            return View(account);
        }

        public ActionResult Partial_History_Order(int id)
        {
            var item = db.Orders.Where(x => x.AccountCode == id).ToList();
            return PartialView(item);
        }
        public ActionResult Partial_Order_Details(int id)
        {

            var item = db.Orders.Find(id);
            List<OrderDetail> list = db.OrderDetails.Where(p => p.OrderCode == id).ToList();
            ViewBag.orderdetail = list;

            return View(item);
        }


        public ActionResult ok(int ordercode)
        {

            List<OrderDetail> list = db.OrderDetails.Where(x => x.OrderCode == ordercode).ToList();



            return View(list);
        }


        public ActionResult GetOrderDetails(int accountCode, int page = 1, int pageSize = 4)
        {
            var orderDetails = db.OrderDetails
                .Where(x => x.Order.AccountCode == accountCode)
                .OrderBy(x => x.OrderCode)
                .ToPagedList(page, pageSize);

            return PartialView("_OrderDetailsPartial", orderDetails);
        }

        public ActionResult MyOrder(int accountCode)
        {
            if (Session["UserId"] != null)
            {
                int userID = (int)Session["UserId"];
                ViewBag.UserId = userID;
            }
            List<Order> order = db.Orders.Where(x => x.AccountCode == accountCode).ToList();
            ViewBag.Order = order;
            List<OrderDetail> list = db.OrderDetails.Where(x => x.Order.AccountCode == accountCode).ToList();
            Account account = db.Accounts.Find(accountCode);
            Session["imgPath"] = account.Avatar;
            return View(list);
        }
        public ActionResult Partial_Inf_Acc(int accountCode)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            Account account = db.Accounts.Find(accountCode);
            Session["imgPath"] = account.Avatar;
            return PartialView("Partial_Inf_Acc", account);
        }

        /*===============================*/
        // Huỳnh Như 19/12/23 11:50 PM
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditProfile(Account account, HttpPostedFileBase uploadFile)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (uploadFile != null)
        //            {
        //                var fileName = Path.GetFileName(uploadFile.FileName);

        //                var path = Path.Combine(Server.MapPath("~/Content/images/comments/"), fileName);

        //                // Convert hình ảnh thành byte array và lưu vào cơ sở dữ liệu
        //                using (var binaryReader = new BinaryReader(uploadFile.InputStream))
        //                {
        //                    account.Avatar = binaryReader.ReadBytes(uploadFile.ContentLength);
        //                }

        //                // Cập nhật đường dẫn hình ảnh mới cho thành viên
        //                db.Entry(account).State = EntityState.Modified;


        //                // Lấy đường dẫn hình ảnh cũ và xóa nếu không còn được sử dụng
        //                string oldImgPath = Request.MapPath(Session["imgPath"].ToString()); // Lấy đường dẫn ảnh (absolute path)

        //                var avatarName = Session["imgPath"].ToString(); // Lấy đường dẫn ảnh (relative path)

        //                // Chuyển đổi sang bytes

        //                byte[] imgBytes = System.IO.File.ReadAllBytes(avatarName);

        //                // Kiểm tra ảnh có trùng với avatar của member nào không
        //                var checkAvatar = db.Accounts.Where(model => model.Avatar == imgBytes).ToList();


        //                // nếu checkavatar là null hoặc không có phần tử nào được tìm thấy trong truy vấn
        //                if (checkAvatar == null)
        //                {
        //                    // lưu trữ hình ảnh mới vào cơ sở dữ liệu
        //                    fileName = Path.GetFileName(uploadFile.FileName);
        //                    path = Path.Combine(Server.MapPath("~/Content/images/comments/"), fileName);

        //                    using (var binaryReader = new BinaryReader(uploadFile.InputStream))
        //                    {
        //                        account.Avatar = binaryReader.ReadBytes(uploadFile.ContentLength);
        //                    }

        //                    db.Entry(account).State = EntityState.Modified;
        //                    db.SaveChanges();
        //                }



        //                if (db.SaveChanges() > 0)
        //                {
        //                    uploadFile.SaveAs(path);
        //                    if (System.IO.File.Exists(oldImgPath) && checkAvatar.Count < 2) // Nếu tồn tại hình trong folder và không member nào có hình này thì xóa ra khỏi folder
        //                    {
        //                        System.IO.File.Delete(oldImgPath);
        //                    }
        //                    // Lấy thông tin mới cập nhập lưu vào session
        //                    var info = db.Accounts.Where(model => model.AccountCode == account.AccountCode).SingleOrDefault();
        //                    Session["info"] = info;
        //                    return RedirectToAction("Index", "Home");
        //                }
        //            }
        //            else
        //            {
        //                if (Session["imgPath"] != null)
        //                {
        //                    string imagePath = Session["imgPath"].ToString();
        //                    byte[] imageBytes = System.IO.File.ReadAllBytes(Server.MapPath(imagePath));
        //                    account.Avatar = imageBytes;
        //                    // Tiếp tục xử lý logic của bạn
        //                }

        //                db.Entry(account).State = EntityState.Modified;

        //                if (db.SaveChanges() > 0)
        //                {
        //                    // Lấy thông tin mới cập nhập lưu vào session
        //                    var info = db.Accounts.Where(model => model.AccountCode == account.AccountCode).SingleOrDefault();
        //                    Session["info"] = info;
        //                    return RedirectToAction("Index", "Home");
        //                }
        //            }
        //        }
        //        ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", account.RoleID);
        //        return View(account);

        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["msgEditProfieFailed"] = "Edit failed! " + ex.Message;
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

    }
}