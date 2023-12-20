//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace electronics_shop.Controllers
//{
//    public class BaseController : Controller
//    {
//        protected override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            var shoppingCart = GetShoppingCartData();
//            // đặt dữ liệu giỏ hàng vòa ViewBaf để có thể sử dụng trong view
//            ViewBag.ShoppingCart = shoppingCart;

//            base.OnActionExecuting(filterContext);
       
//        }

//        public ActionResult YourAction()
//        {
//            // Lấy dữ liệu từ DB hoặc nơi khác
//            IEnumerable<electronics_shop.Models.ShoppingCartItem> shopCart = GetShoppingCartData();

//            // Thêm dữ liệu vào ViewBag
//            ViewBag.ShoppingCart = shopCart;

//            // Trả về View
//            return View();
//        }

//        private IEnumerable<electronics_shop.Models.ShoppingCartItem> GetShoppingCartData()
//        {
//            return new List<electronics_shop.Models.ShoppingCartItem>();
//        }
//    }
//}