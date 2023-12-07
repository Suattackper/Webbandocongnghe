using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count },JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult AddToCart(string id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1,Count = 0 };
            ECOMMERCEEntities db = new ECOMMERCEEntities();
            var checkProduct = db.Products.FirstOrDefault(x => x.ProductCode == id);
                if(checkProduct != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                ShoppingCartItem item = new ShoppingCartItem
                {
                    ProductId = checkProduct.ProductCode,
                    ProductName = checkProduct.ProductName,
                    CategoryName = checkProduct.Category.CategoryName,
                    Quantity = quantity
                };
                //if (checkProduct.ProductImgs.FirstOrDefault(x => x.IsDeFault) != null)
                //{
                //    item.ProductImg = checkProduct.ProductImgs.FirstOrDefault(x => x.IsDeFault).Img;
                //}
                item.Price = (double)checkProduct.Price;
                //if (checkProduct.PromotionCode != null && checkProduct.Promotion.EndDate >= DateTime.Now)
                //{
                //    item.Price = (double)(checkProduct.Price - (checkProduct.Price * checkProduct.Promotion.PromotionPercentage) / 100);
                //}
                item.ToTalPrice = item.Quantity * item.Price;
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart;
                code = new { Success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công", code = 1 ,Count=cart.Items.Count};
            }
            return Json(code);
        }
    }
}