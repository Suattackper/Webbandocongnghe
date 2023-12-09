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
        ECOMMERCEEntities db = new ECOMMERCEEntities();
        public ActionResult Index(/*string id*/)
        {
            //ViewBag.Id = id;
            ViewBag.Offers = db.Products.Where(p => p.Quantity > 0 && db.Promotions.Any(h => h.PromotionCode == p.PromotionCode && h.EndDate >= DateTime.Now)).Take(8).ToList();
            ViewBag.OffersPromotion = db.Promotions.ToList();
            ViewBag.Smarthome = db.Products.Where(p => p.Quantity > 0 && db.Categories.Any(h => h.CategoryCode == p.CategoryCode && h.CategoryName == "Smarthome")).Take(6).ToList();
            ViewBag.Accessory = db.Products.Where(p => p.Quantity > 0 && db.Categories.Any(h => h.CategoryCode == p.CategoryCode && h.CategoryName == "Accessory")).Take(6).ToList();
            ViewBag.GamingGear = db.Products.Where(p => p.Quantity > 0 && db.Categories.Any(h => h.CategoryCode == p.CategoryCode && h.CategoryName == "GamingGear")).Take(6).ToList();
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
            p.Message = message;
            //sử dụng cookie khi đăng nhập
            p.AccountCode = 1;
            db.Contacts.Add(p);
            db.SaveChanges();
            return RedirectToAction("MessageSent", "Account");
        }
    }
}