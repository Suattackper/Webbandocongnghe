using electronics_shop.Models;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

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
            ViewBag.Promotion = db.Promotions.Where(h => h.EndDate >= DateTime.Now && h.StartDate <= DateTime.Now && h.Quantity > 0).ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Brand = db.Brands.ToList();
            if (db.Products == null || db == null)
            {
                return View("Error");
            }
            //List<Product> data = db.Products.ToList();
            List<Product> data = (List<Product>)(from Product in db.Products select Product).ToList();
            return View(data.ToPagedList(page, pagesize));
        }
        //Trang sp loại Smarthome
        public ActionResult Smarthome(int page = 1, int pagesize = 9)
        {
            ViewBag.view = "Smarthome";
            ViewBag.danhmuc = "Smarthome";
            ViewBag.Promotion = db.Promotions.Where(h => h.EndDate >= DateTime.Now && h.StartDate <= DateTime.Now && h.Quantity > 0).ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Brand = db.Brands.ToList();
            if (db.Products == null || db.Categories == null || db == null || db.Promotions == null || db.Brands == null)
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
            ViewBag.Promotion = db.Promotions.Where(h => h.EndDate >= DateTime.Now && h.StartDate <= DateTime.Now && h.Quantity > 0).ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Brand = db.Brands.ToList();
            if (db.Products == null || db.Categories == null || db == null || db.Promotions == null || db.Brands == null)
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
            ViewBag.danhmuc = "Gaming Gear";
            ViewBag.Promotion = db.Promotions.Where(h => h.EndDate >= DateTime.Now && h.StartDate <= DateTime.Now && h.Quantity > 0).ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Brand = db.Brands.ToList();
            if (db.Products == null || db.Categories == null || db == null || db.Promotions == null || db.Brands == null)
            {
                return View("Error");
            }
            List<Product> data = (List<Product>)(from Product in db.Products
                                                 join Category in db.Categories on Product.CategoryCode equals Category.CategoryCode
                                                 where Category.CategoryName == "GamingGear"
                                                 select Product).ToList();
            return View("Shop", data.ToPagedList(page, pagesize));
        }
        //Trang chi tiết sp
        public ActionResult Detail(string id)
        {
            if (db == null || db.Products == null)
            {
                return View("Error");
            }
            Product product = db.Products.FirstOrDefault(p => p.ProductCode == id);
            product.ViewCount = product.ViewCount + 1;
            db.SaveChanges();
            ViewBag.Product = product;
            ViewBag.Promotion = db.Promotions.FirstOrDefault(p => p.PromotionCode == product.PromotionCode && p.EndDate >= DateTime.Now && p.StartDate <= DateTime.Now && p.Quantity > 0);
            ViewBag.productImgsList = db.ProductImgs.Where(h => h.ProductCode == id).ToList();
            ViewBag.Category = db.Categories.FirstOrDefault(p => p.CategoryCode == product.CategoryCode);
            var comment = db.Comments.Where(p => p.ProductCode == id).ToList();
            ViewBag.Comment = comment;
            var account = db.Accounts.ToList();
            ViewBag.Account = account;
            ViewBag.Relate = db.Products.Where(p => p.CategoryCode == product.CategoryCode).ToList();
            ViewBag.RelatePromotion = db.Promotions.Where(h => h.EndDate >= DateTime.Now && h.StartDate <= DateTime.Now && h.Quantity > 0).ToList();
            int rate1 = 0;
            int rate2 = 0;
            int rate3 = 0;
            int rate4 = 0;
            int rate5 = 0;
            if (comment.Count>0)
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
        //tìm kiếm sp
        public ActionResult Search(string s, int page = 1, int pagesize = 9)
        {
            ViewBag.Search = s;
            ViewBag.view = "Search";
            ViewBag.danhmuc = "Shop";
            ViewBag.Promotion = db.Promotions.Where(h => h.EndDate >= DateTime.Now && h.StartDate <= DateTime.Now && h.Quantity > 0).ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Brand = db.Brands.ToList();
            if (db.Products == null || db.Categories == null || db == null || db.Promotions == null || db.Brands == null)
            {
                return View("Error");
            }
            List<Product> data;
            if (s == "" || s == null)
            {
                data = db.Products.ToList();
            }
            else
            {
                data = db.Products.Where(p => p.ProductName.Contains(s)).ToList();
            }
            return View("Shop", data.ToPagedList(page, pagesize));
        }
        //lọc sp
        public ActionResult Fill(string with, string avai, string brand, string min, string max, string search, string view, string danhmuc, int page = 1, int pagesize = 9)
        {
            ViewBag.Search = search;
            ViewBag.view = view;
            ViewBag.Fill = "Fill";
            ViewBag.danhmuc = danhmuc;
            ViewBag.With = with;
            ViewBag.Avai = avai;
            ViewBag.brandd = brand;
            ViewBag.Min = min;
            ViewBag.Max = max;
            ViewBag.Promotion = db.Promotions.Where(h => h.EndDate >= DateTime.Now && h.StartDate <= DateTime.Now && h.Quantity > 0).ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Brand = db.Brands.ToList();
            if (db.Products == null || db.Categories == null || db == null || db.Promotions == null || db.Brands == null)
            {
                return View("Error");
            }
            List<Product> data = db.Products.ToList();
            if (with == "with")
            {
                data = data.Where(p => db.Promotions.Any(h => h.PromotionCode == p.PromotionCode && h.EndDate >= DateTime.Now && h.StartDate <= DateTime.Now && h.Quantity > 0)).ToList();
            }
            if (avai == "avai")
            {
                data = data.Where(p => p.Quantity > 0).ToList();
            }
            if (brand != "ALL")
            {
                data = data.Where(p => db.Brands.Any(h => h.BrandCode == p.BrandCode && h.BrandName == brand)).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(p => p.ProductName.Contains(search)).ToList();
            }
            if (view != "Shop")
            {
                data = data.Where(p => db.Categories.Any(h => h.CategoryCode == p.CategoryCode && h.CategoryName == view)).ToList();
            }
            if (!string.IsNullOrEmpty(min) && !string.IsNullOrEmpty(max))
            {
                decimal Min = decimal.Parse(min);
                decimal Max = decimal.Parse(max);
                //data = data.Where(p => p.Price >= Min && p.Price <= Max).ToList();
                List<Product> d1 = data.Join(db.Promotions, product => product.PromotionCode, promotion => promotion.PromotionCode, (product, promotion) => new { product, promotion })
                            .Where(x => (x.promotion.EndDate >= DateTime.Now && x.promotion.StartDate <= DateTime.Now && x.promotion.Quantity > 0 && (x.product.Price - x.product.Price * x.promotion.PromotionPercentage / 100) >= Min && (x.product.Price - x.product.Price * x.promotion.PromotionPercentage / 100) <= Max) || ((x.promotion.EndDate < DateTime.Now || x.promotion.StartDate > DateTime.Now || x.promotion.Quantity <= 0) && x.product.Price >= Min && x.product.Price <= Max))
                            .Select(x => x.product)
                            .ToList();
                List<Product> d2 = data.Where(p => p.Promotion == null && p.Price >= Min && p.Price <= Max).ToList();
                data = d1.Concat(d2).ToList();
                //data = data.Join(db.Promotions, product => product.PromotionCode, promotion => promotion.PromotionCode, (product, promotion) => new { product, promotion })
                //            .Where(x => (x.promotion == null && x.product.Price >= Min && x.product.Price <= Max) || (x.promotion != null && x.promotion.EndDate >= DateTime.Now && (x.product.Price - x.product.Price * x.promotion.PromotionPercentage / 100) >= Min && (x.product.Price - x.product.Price * x.promotion.PromotionPercentage / 100) <= Max) || (x.promotion != null && x.promotion.EndDate < DateTime.Now && x.product.Price >= Min && x.product.Price <= Max))
                //            .Select(x => x.product)
                //            .ToList();
            }
            data = data.ToList();
            return View("Shop", data.ToPagedList(page, pagesize));
        }
        //add review
        [HttpPost]
        public ActionResult Review(string ratingvalue, string reviewvalue, string productcode, string accountcode)
        {
            Comment c = new Comment();
            c.AccountCode = int.Parse(accountcode);
            c.ProductCode = productcode;
            c.CommentContent = reviewvalue;
            c.CommentTime = DateTime.Now;
            c.Rate = double.Parse(ratingvalue);
            db.Comments.Add(c);
            db.SaveChanges();
            List<Comment> data = db.Comments.Where(p => p.ProductCode == productcode).ToList();
            double ratep = 0;
            foreach (var i in data)
            {
                ratep = ratep + (double)i.Rate;
            }
            ratep = ratep / data.Count;
            Product product = db.Products.FirstOrDefault(p => p.ProductCode == productcode);
            product.Rate = ratep;
            db.SaveChanges();
            return RedirectToAction("Detail", new { id = productcode });
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
                return PartialView("_RenderShoppingCart", cart.Items);
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
    }
}