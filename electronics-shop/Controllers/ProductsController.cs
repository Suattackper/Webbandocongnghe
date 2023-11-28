using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Controllers
{
    public class ProductsController : Controller
    {
        ECOMMERCEEntities1 db = new ECOMMERCEEntities1();
        // GET: Products
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Shop()
        {
            ViewBag.view = "Shop";
            ViewBag.danhmuc = "Shop";
            if (db.Products == null || db == null)
            {
                return View("Error");
            }
            //List<Product> data = db.Products.ToList();
            List<Product> data = (List<Product>)(from Product in db.Products select Product).ToList();
            return View(data);
        }
        public ActionResult Smarthome()
        {
            ViewBag.view = "Smarthome";
            ViewBag.danhmuc = "Smarthome";
            if (db.Products == null || db.Categories == null || db == null)
            {
                return View("Error");
            }
            List<Product> data = (List<Product>)(from Product in db.Products
                                                 join Category in db.Categories on Product.CategoryCode equals Category.CategoryCode
                                                 where Category.CategoryName == "smarthome"
                                                 select Product).ToList();
            return View("Shop",data);
        }
        public ActionResult Keyboard()
        {
            ViewBag.view = "Keyboard";
            ViewBag.danhmuc = "Keyboard";
            if (db.Products == null || db.Categories == null || db == null)
            {
                return View("Error");
            }
            List<Product> data = (List<Product>)(from Product in db.Products
                                                 join Category in db.Categories on Product.CategoryCode equals Category.CategoryCode
                                                 where Category.CategoryName == "keyboard"
                                                 select Product).ToList();
            return View("Shop", data);
        }
        public ActionResult ComputerAccessories()
        {
            ViewBag.view = "ComputerAccessories";
            ViewBag.danhmuc = "Computer Accessories";
            if (db.Products == null || db.Categories == null || db == null)
            {
                return View("Error");
            }
            List<Product> data = (List<Product>)(from Product in db.Products
                                                 join Category in db.Categories on Product.CategoryCode equals Category.CategoryCode
                                                 where Category.CategoryName == "computer accessories"
                                                 select Product).ToList();
            return View("Shop", data);
        }
        public ActionResult Detail()
        {
            return View();
        }
    }
}