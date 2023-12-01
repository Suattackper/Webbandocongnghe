using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace electronics_shop.Controllers
{
    public class HomeController : Controller
    {
        ECOMMERCEEntities1 db = new ECOMMERCEEntities1();
        public ActionResult Index(/*string id*/)
        {
            //ViewBag.Id = id;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page."; 

            return View();
        }
        [HttpPost]
        public ActionResult About(string username, string password)
        {
            // Kiểm tra username và password, thực hiện xác thực người dùng
            if (username=="bb" && password=="bb")
            {
                // Đăng nhập thành công, thực hiện chuyển hướng hoặc các logic khác
                //Session["id"] = username;
                return RedirectToAction("Index", "Home", new {id = username});
            }
            else
            {
                // Đăng nhập thất bại, hiển thị thông báo hoặc thực hiện các xử lý khác
                ViewBag.ErrorMessage = "Đăng nhập thất bại";
                return View();
            }
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(string name, string email, string message)
        {
            Contact p = new Contact();
            p.FullName = name;
            p.Email = email;
            p.Message = message ;
            p.AccountCode = 1;
            db.Contacts.Add(p);
            db.SaveChanges();
            return RedirectToAction("MessageSent", "Account");
        }
    }
}