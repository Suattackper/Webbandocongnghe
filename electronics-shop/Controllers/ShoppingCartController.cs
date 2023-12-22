using electronics_shop.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Controllers
{
    public class ShoppingCartController : Controller
    {
       
       private ECOMMERCE5Entities db = new ECOMMERCE5Entities();
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

        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.checkCart = cart;
               
            }
            return View();
        }
        //thanh toán thành công
        public ActionResult CheckOutSuccess()
        {
            return View();
        }

        public ActionResult RenderInfCus()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                ViewBag.checkCart = cart;
                return PartialView("RenderInfCustomer",cart.Items);
            }
            return PartialView("RenderInfCustomer");
        }



        public ActionResult toLogin()
        {
            if (Session["info"] == null)
            {
                return RedirectToAction("login", "Account");

            }
            return View();
        }

        public ActionResult Patial_CheckOut()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOutForm(OrderViewModel req)
           {
            var code = new { Success = false, Code = -1 };
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    Order order = new Order();
                    order.Account = new Account();
                    order.AccountAddress = new AccountAddress();
                        order.Account.FirstName = req.FirstName;

                    
                    order.Account.LastName = req.LastName;
                    order.Account.Email = req.Email;
                    order.Account.PhoneNumber = req.PhoneNumber;
                    order.AccountAddress.Number = req.Number;
                    order.AccountAddress.District = req.District;
                    order.AccountAddress.Ward = req.Ward;
                    order.AccountAddress.City = req.City;
                    cart.Items.ForEach(x => order.orderDetails.Add(new OrderDetail { ProductCode = x.ProductId,
                                Quantity = x.Quantity,
                                Price = (decimal?)x.Price
                    }));
                    order.OrderTotal = (decimal?)cart.Items.Sum(x => (x.Price * x.Quantity));
                    //order.typepayment = req.typepayment;
                    order.OrderDate = DateTime.Now;
                    
                    //order.CreateBy = req.PhoneNumber;
                    Random rd = new Random();
                    order.OrderCode = rd.Next(0,9) + rd.Next(0, 9) + rd.Next(0, 9);
                    db.Orders.Add(order);
                    db.SaveChanges();
                    // send mail cho khach hang
                    var strSanPham = "";
                    var thanhtien = decimal.Zero;
                    var TongTien = decimal.Zero;
                    foreach(var sp in cart.Items)
                    {
                        strSanPham += "<tr>";
                        strSanPham += "<td>"+sp.ProductName+"</td>";
                        strSanPham += "<td>"+sp.Quantity+"</td>";
                        strSanPham += "<td>"+ string.Format("₫{0:#,0}",sp.ToTalPrice) + "</td>";
                        strSanPham += "</tr>";
                        thanhtien += (decimal)(sp.Price * sp.Quantity);
                    }
                    TongTien = thanhtien; //+vanchuyen
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}",order.OrderCode.ToString());
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.Account.FirstName +" "+ order.Account.LastName);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", order.Account.PhoneNumber);
                    contentCustomer = contentCustomer.Replace("{{Email}}", order.Account.Email);
                    contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}",order.AccountAddress.Number + " " + order.AccountAddress.Ward + " " + order.AccountAddress.District + " " + order.AccountAddress.City);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", order.OrderDate.ToString());
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", string.Format("₫{0:#,0}", thanhtien));
                    contentCustomer = contentCustomer.Replace("{{TongTien}}", string.Format("₫{0:#,0}", TongTien));
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    electronics_shop.Common.Common.SendMail("BenzikShop","Đơn hàng #"+order.OrderCode,contentCustomer.ToString(),req.Email);

                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                    contentAdmin  = contentAdmin.Replace("{{MaDon}}", order.OrderCode.ToString());
                    contentAdmin  = contentAdmin.Replace("{{SanPham}}", strSanPham);
                    contentAdmin  = contentAdmin.Replace("{{TenKhachHang}}", order.Account.FirstName + " " + order.Account.LastName);
                    contentAdmin  = contentAdmin.Replace("{{Phone}}", order.Account.PhoneNumber);
                    contentAdmin  = contentAdmin.Replace("{{Email}}", order.Account.Email);
                    contentAdmin  = contentAdmin.Replace("{{SoNha+Phuong}}", order.AccountAddress.Number + " " + order.AccountAddress.Ward);
                    contentAdmin  = contentAdmin.Replace("{{Tinh}}", order.AccountAddress.District);
                    contentAdmin  = contentAdmin.Replace("{{ThanhPho}}", order.AccountAddress.City);
                    contentAdmin  = contentAdmin.Replace("{{DiaChiNhanHang}}", order.AccountAddress.Number);
                    contentAdmin  = contentAdmin.Replace("{{NgayDat}}", order.OrderDate.ToString());
                    contentAdmin  = contentAdmin.Replace("{{ThanhTien}}", string.Format("₫{0:#,0}", thanhtien));
                    contentAdmin  = contentAdmin.Replace("{{TongTien}}", string.Format("₫{0:#,0}", TongTien));
                    contentAdmin  = contentAdmin.Replace("{{SanPham}}", strSanPham); 
                    electronics_shop.Common.Common.SendMail("BenzikShop", "Đơn hàng mới #" + order.OrderCode, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"] );
                    //code = new { Success = true, Code = 1 }
                    cart.ClearCart();
                    return RedirectToAction("CheckOutSuccess");
                }
            }
            return Json(code);
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