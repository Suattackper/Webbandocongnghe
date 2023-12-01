using electronics_shop.Models;
using PagedList;
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
        public ActionResult Shop(int page = 1, int pagesize = 9)
        {
            ViewBag.view = "Shop";
            ViewBag.danhmuc = "Shop";
            if (db.Products == null || db == null)
            {
                return View("Error");
            }
            //List<Product> data = db.Products.ToList();
            List<Product> data = (List<Product>)(from Product in db.Products select Product).ToList();
            return View(data.ToPagedList(page,pagesize));
        }
        public ActionResult Smarthome(int page = 1, int pagesize = 9)
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
            return View("Shop", data.ToPagedList(page, pagesize));
        }
        public ActionResult Keyboard(int page = 1, int pagesize = 9)
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
            return View("Shop", data.ToPagedList(page, pagesize));
        }
        public ActionResult ComputerAccessories(int page = 1, int pagesize = 9)
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
            return View("Shop", data.ToPagedList(page, pagesize));
        }
        public ActionResult Detail(string id)
        {
            if (db == null || db.Products == null)
            {
                return View("Error");
            }

            ProductDetail data = new ProductDetail();
            data.Product = db.Products.FirstOrDefault(p => p.ProductCode == id);
            data.productImgsList = db.ProductImgs.Where(h => h.ProductCode == id).ToList();
            return View(data);


            //if (db.Products == null || db == null)
            //{
            //    return View("Error");
            //}
            ////List<Product> data = db.Products.ToList();
            //Product data = (Product)(from Product in db.Products where Product.ProductCode == id select Product);
            //return View(data);
        }
    }
}