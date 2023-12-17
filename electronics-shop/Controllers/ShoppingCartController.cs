using electronics_shop.Models;
using Microsoft.Ajax.Utilities;
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
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if(cart != null)
            {
                return View(cart.Items);
            }
            return View();
        }

        public ActionResult Checkout()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return View(cart.Items);
            }
            return View();
        }
        public ActionResult Partial_Item_Cart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return PartialView("_Partial_Item_Cart", cart.Items);
            }
            return PartialView("_Partial_Item_Cart");
        }

        [HttpGet]
        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count },JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult toLogin()
        {
            if (Session["info"] == null)
            {
                return RedirectToAction("login", "Account");

            }
            return View();
        }
        [HttpPost]
        public ActionResult AddToCart(string id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1,Count = 0 };
            var erAc = new { Success = true, msg = "" };
            ECOMMERCE5Entities db = new ECOMMERCE5Entities();
            var checkProduct = db.Products.FirstOrDefault(x => x.ProductCode == id);
          
            if (Session["info"] == null)
            {
                erAc = new { Success = false, msg = "Vui lòng đăng nhập trước khi mua hàng" };
                    ShoppingCart cart = (ShoppingCart)Session["Cart"];
                
                return Json(erAc);
            }
            else
            {
                if (checkProduct != null)
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
                    if (checkProduct.ProductImgs.FirstOrDefault(x => x.IsDeFault) == null)
                    {
                        item.ProductImg = checkProduct.ImageProduct;
                    }
                    item.Price = (double)checkProduct.Price;
                    if (checkProduct.PromotionCode != null && checkProduct.Promotion.EndDate >= DateTime.Now)
                    {
                        item.PromotionPrice = item.Price;
                        item.Price = (double)(checkProduct.Price - (checkProduct.Price * checkProduct.Promotion.PromotionPercentage) / 100);

                    }
                    else
                    {
                        item.PromotionPrice = 0;
                    }
                    item.ToTalPrice = item.Quantity * item.Price;
                    cart.AddToCart(item, quantity);
                    Session["Cart"] = cart;
                    code = new { Success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công", code = 1, Count = cart.Items.Count };
                }
                return Json(code);
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, msg = "Đã xóa thành công", code = 1, Count = cart.Items.Count };
                }

            }
            return Json(code);
        }



        [HttpPost]
        public ActionResult Update(string id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.UpdateQuantity(id,quantity);
                return Json(new { Success = true, Count = 0 });
            }
            return Json(new { Success = false, Count = cart.Items.Count });
        }
    

    [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
                return Json(new {Success=true,Count =0});
            }
            return Json(new {Success = false,Count = cart.Items.Count});
        }
    }
}