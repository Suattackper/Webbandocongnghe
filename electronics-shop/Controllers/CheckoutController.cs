//using electronics_shop.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace electronics_shop.Controllers
//{
//    public class CheckoutController : Controller
//    {
//        // GET: Checkout
//        public ActionResult Index()
//        {
//                ShoppingCart cart = (ShoppingCart)Session["Cart"];
//                if (cart != null)
//                {
//                    return View(cart.Items);
//                }
//                return View();
//         }
//        public ActionResult RenderInfCus()
//        {
//            ShoppingCart cart = (ShoppingCart)Session["Cart"];
//            if (cart != null)
//            {
//                ViewBag.checkCart = cart;
//                return PartialView("_Partial_Inf_Cus");
//            }
//            return PartialView("_Partial_Inf_Cus");
//        }
//    }
//}