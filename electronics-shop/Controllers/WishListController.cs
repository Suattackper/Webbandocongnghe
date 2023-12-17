using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Controllers
{
    public class WishListController : Controller
    {
        // GET: WishList
     
            // GET: Cart
            public ActionResult Index()
            {
            WishListCart cart = (WishListCart)Session["Wish"];
                if (cart != null)
                {
                    return View(cart.Items);
                }
                return View();
            }

            public ActionResult Checkout()
            {
            WishListCart cart = (WishListCart)Session["Wish"];
                if (cart != null)
                {
                    return View(cart.Items);
                }
                return View();
            }
            public ActionResult Partial_Wish_Cart()
            {
            WishListCart cart = (WishListCart)Session["Wish"];
                if (cart != null)
                {
                    return PartialView("_Partial_Wish_Cart", cart.Items);
                }
                return PartialView("_Partial_Wish_Cart");
            }

            [HttpGet]
            public ActionResult ShowCount()
            {
            WishListCart cart = (WishListCart)Session["Wish"];
                if (cart != null)
                {
                    return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);

            }

            [HttpPost]
            public ActionResult AddToWishList(string id, int quantity)
            {
                var code = new { Success = false, msg = "", code = -1, Count = 0 };
            ECOMMERCE5Entities db = new ECOMMERCE5Entities();
                var checkProduct = db.Products.FirstOrDefault(x => x.ProductCode == id);
                if (checkProduct != null)
                {
                WishListCart cart = (WishListCart)Session["Wish"];
                    if (cart == null)
                    {
                        cart = new WishListCart();
                    }
                WishListItem item = new WishListItem
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
                    Session["Wish"] = cart;
                    code = new { Success = true, msg = "Thêm sản phẩm vào yêu thích thành công", code = 1, Count = cart.Items.Count };
                }
                return Json(code);
            }

            [HttpPost]
            public ActionResult Delete(string id)
            {
                var code = new { Success = false, msg = "", code = -1, Count = 0 };

            WishListCart cart = (WishListCart)Session["Wish"];
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
            WishListCart cart = (WishListCart)Session["Wish"];
                if (cart != null)
                {
                    cart.UpdateQuantity(id, quantity);
                    return Json(new { Success = true, Count = 0 });
                }
                return Json(new { Success = false, Count = cart.Items.Count });
            }


            [HttpPost]
            public ActionResult DeleteAll()
            {
            WishListCart cart = (WishListCart)Session["Wish"];
                if (cart != null)
                {
                    cart.ClearWish();
                    return Json(new { Success = true, Count = 0 });
                }
                return Json(new { Success = false, Count = cart.Items.Count });
            }
        }
    
}