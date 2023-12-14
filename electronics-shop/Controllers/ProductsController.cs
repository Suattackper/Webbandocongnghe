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
        ECOMMERCEEntities db = new ECOMMERCEEntities();
        // GET: Products
        public ActionResult Index()
        {
            return View();
        }
        //Trang sp
        public ActionResult Shop(int page = 1, int pagesize = 9)
        {
         
          
            ViewBag.view = "Shop";
            ViewBag.danhmuc = "Shop";
            ViewBag.Promotion = db.Promotions.ToList();
            if (db.Products == null || db == null)
            {
                return View("Error");
            }
            //List<Product> data = db.Products.ToList();
  
            

            List<Product> data = (List<Product>)(from Product in db.Products select Product).ToList();
            return View(data.ToPagedList(page,pagesize));
        }

        public ActionResult RenderShoppingCart()
        {
            //var shoppingCart = GetShoppingCartData();
            //if (shoppingCart != null)
            //{
            //    ViewBag.ShoppingCart = shoppingCart;

            //}
            //else
            //{
            //    return View("Error");
            //}
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return PartialView("_RenderShoppingCart",cart.Items);
            }
            return PartialView("_RenderShoppingCart");
        }

    

      
        public ActionResult RenderActionCart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];

            if (cart != null)
            {
             
                

                return PartialView("_ActionViewCart", cart.Items);
            }

            return PartialView("_ActionViewCart");
        }

        private IEnumerable<electronics_shop.Models.ShoppingCartItem> GetShoppingCartData()
        {
            if (Session["CartShop"] is List<electronics_shop.Models.ShoppingCartItem> carts)
            {
                return carts;
            }
            carts = new List<ShoppingCartItem>();
            Session["CartShop"] = carts;
            return carts;
        }


        //Trang sp loại Smarthome
        public ActionResult Smarthome(int page = 1, int pagesize = 9)
        {
            ViewBag.view = "Smarthome";
            ViewBag.danhmuc = "Smarthome";
            ViewBag.Promotion = db.Promotions.ToList();
            if (db.Products == null || db.Categories == null || db == null)
            {
                return View("Error");
            }
            List<Product> data = (List<Product>)(from Product in db.Products
                                                 join Category in db.Categories on Product.CategoryCode equals Category.CategoryCode
                                                 where Category.CategoryName == "Smarthome"
                                                 select Product).ToList();
            return View("Shop", data.ToPagedList(page, pagesize));
        }
        //Trang sp loại Accessory
        public ActionResult Accessory(int page = 1, int pagesize = 9)
        {
            ViewBag.view = "Accessory";
            ViewBag.danhmuc = "Phụ kiện LapTop - PC";
            ViewBag.Promotion = db.Promotions.ToList();
            if (db.Products == null || db.Categories == null || db == null)
            {
                return View("Error");
            }
            List<Product> data = (List<Product>)(from Product in db.Products
                                                 join Category in db.Categories on Product.CategoryCode equals Category.CategoryCode
                                                 where Category.CategoryName == "Accessory"
                                                 select Product).ToList();
            return View("Shop", data.ToPagedList(page, pagesize));
        }
        //Trang sp loại GamingGear
        public ActionResult GamingGear(int page = 1, int pagesize = 9)
        {
            ViewBag.view = "GamingGear";
            ViewBag.danhmuc = "GamingGear";
            ViewBag.Promotion = db.Promotions.ToList();
            if (db.Products == null || db.Categories == null || db == null)
            {
                return View("Error");
            }
            List<Product> data = (List<Product>)(from Product in db.Products
                                                 join Category in db.Categories on Product.CategoryCode equals Category.CategoryCode
                                                 where Category.CategoryName == "GamingGear"
                                                 select Product).ToList();
            return View("Shop", data.ToPagedList(page, pagesize));
        }

        public dynamic GetViewBag()
        {
            return ViewBag;
        }




        //Trang chi tiết sp
        public ActionResult Detail(string id)
        {
            if (db == null || db.Products == null)
            {
                return View("Error");
            }
            //List<Product> sanpham = db.Products.ToList();
            //ProductDetail data = new ProductDetail();
            //data.Product = db.Products.FirstOrDefault(p => p.ProductCode == id);
            //data.productImgsList = db.ProductImgs.Where(h => h.ProductCode == id).ToList();
            Product product = db.Products.FirstOrDefault(p => p.ProductCode == id);
            ViewBag.Product = product;
            ViewBag.Promotion = db.Promotions.FirstOrDefault(p => p.PromotionCode == product.PromotionCode && p.EndDate >= DateTime.Now);
            List<ProductImg> productImgs = db.ProductImgs.Where(h => h.ProductCode == id).ToList();
            ViewBag.productImgsList = productImgs;
            Category category = db.Categories.FirstOrDefault(p => p.CategoryCode == product.CategoryCode);
            ViewBag.Category = category;
            var comment = db.Comments.Where(p => p.ProductCode == id).ToList();
            ViewBag.Comment = comment;
            var account = db.Accounts.ToList();
            ViewBag.Account = account;
            ViewBag.Relate = db.Products.Where(p => p.CategoryCode == product.CategoryCode).ToList();
            ViewBag.RelatePromotion = db.Promotions.ToList();
            int rate1 = 0;
            int rate2 = 0;
            int rate3 = 0;
            int rate4 = 0;
            int rate5 = 0;
            if (comment.Count > 0)
            {
                foreach (var c in comment)
                {
                    if (c.Rate == 1) rate1++;
                    else if (c.Rate == 2) rate2++;
                    else if (c.Rate == 3) rate3++;
                    else if (c.Rate == 4) rate4++;
                    else rate5++;
                }
                ViewBag.Rate1 = (double)rate1 / comment.Count * 100;
                ViewBag.Rate2 = (double)rate2 / comment.Count * 100;
                ViewBag.Rate3 = (double)rate3 / comment.Count * 100;
                ViewBag.Rate4 = (double)rate4 / comment.Count * 100;
                ViewBag.Rate5 = (double)rate5 / comment.Count * 100;
            }
            else
            {
                ViewBag.Rate1 = rate1;
                ViewBag.Rate2 = rate2;
                ViewBag.Rate3 = rate3;
                ViewBag.Rate4 = rate4;
                ViewBag.Rate5 = rate5;
            }
            //return View(data);
            return View(product);
        }
    }
}