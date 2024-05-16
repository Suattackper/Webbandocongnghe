using electronics_shop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.admin.Controllers
{
    public class ContactsController : Controller
    {
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        // GET: admin/Contacts
        public ActionResult Index(int page = 1, int pagesize = 10)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            List<Contact> data = db.Contacts.OrderByDescending(p => p.ContactCode).ToList();
            return View(data.ToPagedList(page, pagesize));
        }
        [HttpGet]
        public ActionResult Search(string search, int page = 1, int pagesize = 10)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            ViewBag.Search = search;
            List<Contact> data = db.Contacts.OrderByDescending(p => p.ContactCode).Where(p => p.Email.Contains(search) || p.FullName.Contains(search)).ToList();
            return View("Index", data.ToPagedList(page, pagesize));
        }
        public ActionResult Reply(string id)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            int code = int.Parse(id);
            Contact brand = db.Contacts.FirstOrDefault(p => p.ContactCode == code);
            return View(brand);
        }
        [HttpPost]
        public ActionResult Reply(string id, string message)
        {
            int code = int.Parse(id);
            Contact brand = db.Contacts.FirstOrDefault(p => p.ContactCode == code);
            brand.Status = true;
            db.SaveChanges();
            //xử lý gửi mail ở đây
            // Thông tin tài khoản email
            string fromEmail = "baitoan88@gmail.com";
            string password = "tkea vcgz hdvj rjmw";

            // Địa chỉ người nhận
            string toEmail = brand.Email;

            // Tạo đối tượng MailMessage
            MailMessage mail = new MailMessage(fromEmail, toEmail);

            // Tiêu đề và nội dung email
            mail.Subject = "Besnik. reply to question!";
            mail.Body = $"Xin chào {brand.FullName}, cảm ơn bạn đã sử dụng web Besnik của chúng tôi! \nChúng tôi đã nhận câu hỏi của bạn vào lúc {brand.ContactDate}: \n\t- {brand.Message}\nĐây là câu trả lời của chúng tôi gửi đến bạn:\n\t- {message}\nHi vọng rằng vấn đề của bạn đã được giải quyết!";

            // Cấu hình đối tượng SmtpClient
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587; // Port thường là 587 cho SMTP over TLS (SSL)
            smtpClient.Credentials = new NetworkCredential(fromEmail, password);
            smtpClient.EnableSsl = true;

            try
            {
                // Gửi email
                smtpClient.Send(mail);
                //ViewBag.success = "Email sent successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error -- " + ex.Message + "!";
                return RedirectToAction("Reply", new {id = id});
            }
        }
        public ActionResult Delete(string id)
        {
            int code = int.Parse(id);
            Contact brand = db.Contacts.FirstOrDefault(p => p.ContactCode == code);
            if (brand == null)
            {
                // Trường hợp không tìm thấy thương hiệu, trả về thông báo lỗi
                TempData["Error"] = "Error -- Không tìm thấy câu hỏi để xóa!";
                return RedirectToAction("Index");
            }
            if (brand.Status == false)
            {
                // Trường hợp chưa phản hồi, trả về thông báo lỗi
                TempData["Error"] = $"Error -- Câu hỏi có mã {id} chưa được phản hồi!";
                return RedirectToAction("Index");
            }
            db.Contacts.Remove(brand);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}