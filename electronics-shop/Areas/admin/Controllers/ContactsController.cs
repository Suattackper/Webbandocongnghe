using electronics_shop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
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



            return RedirectToAction("Index");
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