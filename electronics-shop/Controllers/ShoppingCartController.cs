using electronics_shop.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices.CompensatingResourceManager;
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
            if (TempData.ContainsKey("promotioncode"))
            {
                string s = (string)TempData["promotioncode"];
                Promotion ma = db.Promotions.FirstOrDefault(p => p.PromotionCode == s && p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now && p.Quantity > 0);
                
                ViewBag.promotioncode = ma;
            }
            if (TempData.ContainsKey("error"))
            {
                ViewBag.error = (string)TempData["error"];
            }
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if(cart != null)
            {
                return View(cart.Items);
            }
            return View();
        }

      
        public ActionResult Partial_Item_Cart(/*string promotioncode*/)
        {
            //string code = "";
            //if(promotioncode != "")
            //{
            //    code = promotioncode;
            //    Session["magiamgia"] = promotioncode;
            //}
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
        public ActionResult CheckOut(decimal total, string promotion)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            ViewBag.total = total;
            if (cart != null && cart.Items.Any())
            {
                ViewBag.checkCart = cart;
                if (Session["info"] != null)
                {
                    int userID = (int)Session["UserId"];
                    var account = db.Accounts.FirstOrDefault(a => a.AccountCode == userID);

                    ViewBag.FirstName = account.FirstName;
                    ViewBag.LastName = account.LastName;
                    ViewBag.Email = account.Email;
                    ViewBag.Phone = account.PhoneNumber;
                    ViewBag.total = total;
                    ViewBag.promotion = promotion;
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult CheckOutPromotion(string promotioncode)
        {
            Promotion check = db.Promotions.FirstOrDefault(i => i.PromotionCode == promotioncode && i.StartDate <= DateTime.Now && i.EndDate >= DateTime.Now && i.Quantity > 0);
            if(check != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];

                if (cart != null && cart.Items.Any())
                {
                    ViewBag.checkCart = cart;

                }
                TempData["promotioncode"] = promotioncode;
                return RedirectToAction("Index");
            }
            else
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];

                if (cart != null && cart.Items.Any())
                {
                    ViewBag.checkCart = cart;
                }
                TempData["error"] = "Mã không hợp lệ!";
                return RedirectToAction("Index");
            }
            //return View();
        }
        //thanh toán thành công
        public ActionResult CheckOutSuccess()
        {
            Order item = new Order();
            

            return View(item);
        }

        public ActionResult RenderInfCus(decimal total)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                ViewBag.total = total;
                ViewBag.checkCart = cart;
                return View(cart.Items);
            }
            return View();
        }



        public ActionResult toLogin()
        {
            if (Session["info"] == null)
            {
                return RedirectToAction("login", "Account");

            }
            return View();
        }

        
        public ActionResult Patial_CheckOut(decimal total)
        {
           if (Session["info"] != null)
            {
                int userID = (int)Session["UserId"];
                var account = db.Accounts.FirstOrDefault(a=>a.AccountCode==userID);

                ViewBag.FirstName = account.FirstName;
                ViewBag.LastName = account.LastName;
                ViewBag.Email = account.Email;
                ViewBag.Phone = account.PhoneNumber;
                ViewBag.total = total;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOutForm(OrderViewModel req, string total, string promotion)
           {
            var code = new { Success = false, Code = -1 };
           

            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                string magiamgia = "";
                int? mucgiam = 0;
                if (promotion != null && promotion != "")
                {
                    //string x = (string)Session["promotioncode"];
                    Promotion p = db.Promotions.FirstOrDefault(j => j.PromotionCode == promotion);
                    magiamgia = p.PromotionCode;
                    mucgiam = p.PromotionPercentage;
                }
                if (cart != null)
                {
                    Order order = new Order();
                    //Product product = new Product();
                    order.AccountAddress = new AccountAddress();

                    if (Session["info"] != null)
                    {

                        int userID = (int)Session["UserID"];
                        var account = db.Accounts.FirstOrDefault(a => a.AccountCode == userID);
                        order.Account = account;

                        order.Account.FirstName = account.FirstName;

                        order.Account.LastName = account.LastName;
                        order.Account.Email = account.Email;
                        order.AccountAddress.PhoneNumber = account.PhoneNumber;
                    }
                    if (magiamgia != "")
                    {
                        order.PromotionCode = magiamgia;
                        Promotion c = db.Promotions.FirstOrDefault(p => p.PromotionCode == magiamgia );
                        c.Quantity--;
                        db.SaveChanges();
                    }
                    order.DeliveryCode = "GHN";
                    
                    order.AccountAddress.Number = req.Number;   
                    order.AccountAddress.District = req.District;
                    order.AccountAddress.Ward = req.Ward;
                    order.AccountAddress.City = req.City;
                    
                    order.AccountCode = (int)Session["UserId"] ; // chỗ này nhập accountcode đã có trong csdl của m, hoặc truyên fvaof khi đăng nhập thành công
                    order.AccountAddressCode = order.AccountAddressCode;  // chỗ này tạo đại 1 accountaddress trong csdl r nhập accountaddresscode vào
                    order.PaymentCode = req.TypePayment;
                   // chỗ này tạo đại 1 payment trong csdl r nhập PaymentCode vào
                    order.OrderDate = DateTime.Now;
                    order.OrderTotal = decimal.Parse(total); //(decimal?)cart.Items.Sum(x => (x.Price * x.Quantity));   // chỗ này nhập tổng bill vào
                    order.OrderNote = "";  // chỗ này nhập ghi chú vaof 
                    order.Delivered = false; //chưa vận chuyển
                    //product.Quantity = product.Quantity - cart.Items.Sum(x=>(x.Quantity));
                    //db.SaveChanges();
                    db.Orders.Add(order);
                    db.SaveChanges();
                    ViewBag.TypePayment = order.PaymentCode;
                    //lấy mã ordercode
                    Order ordercode = db.Orders.OrderByDescending(o => o.OrderCode).FirstOrDefault();


                    //cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetail
                    //{
                    //    OrderCode = ordercode.OrderCode,
                    //    ProductCode = x.ProductId,
                    //    Quantity = x.Quantity,
                    //    Price = (decimal?)x.Price
                    //}));
                    foreach(var i in cart.Items)
                    {
                        OrderDetail p = new OrderDetail();
                        p.ProductCode = i.ProductId;
                        p.Quantity = i.Quantity;
                        p.Price = decimal.Parse(i.Price.ToString());
                        p.OrderCode = order.OrderCode;
                        p.Total = decimal.Parse((i.Price * i.Quantity).ToString());

                        Product f = db.Products.FirstOrDefault(j => j.ProductCode == i.ProductId);
                        List<Promotion> check = db.Promotions.Where(h => h.StartDate <= DateTime.Now && h.EndDate >= DateTime.Now && h.Quantity > 0).ToList();
                        foreach(var item in check)
                        {
                            if(f.PromotionCode == item.PromotionCode)
                            {
                                Promotion n = db.Promotions.FirstOrDefault(x => x.PromotionCode == item.PromotionCode);
                                n.Quantity--;
                                db.SaveChanges();
                                p.Total = decimal.Parse((p.Total - p.Total * n.PromotionPercentage / 100).ToString());
                                break;
                            }

                        }
                        db.OrderDetails.Add(p);
                        db.SaveChanges();

                    }
                    var listorderdetail = db.OrderDetails.Where(p => p.OrderCode == ordercode.OrderCode).ToList();
                    foreach(var i in listorderdetail)
                    {
                        Product p = db.Products.FirstOrDefault(h => h.ProductCode == i.ProductCode);
                        p.Quantity = p.Quantity - i.Quantity;
                        db.SaveChanges();
                    }

                    //order.OrderTotal = (decimal?)cart.Items.Sum(x => (x.Price * x.Quantity));
                    //order.typepayment = req.typepayment;
                    //order.OrderDate = DateTime.Now;

                    //order.CreateBy = req.PhoneNumber;
                    //Random rd = new Random();
                    //order.OrderCode = rd.Next(0,9) + rd.Next(0, 9) + rd.Next(0, 9);

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
                    if(mucgiam != 0)
                    {
                        TongTien = thanhtien - thanhtien * (decimal)mucgiam / 100 + 30000; //+vanchuyen
                    }
                    else TongTien = thanhtien + 30000; //+vanchuyen
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}", order.OrderCode.ToString());
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    if (Session["info"] != null)
                    {
                        int userID = (int)Session["UserID"];
                        var account = db.Accounts.FirstOrDefault(a => a.AccountCode == userID);
                        contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", account.FirstName + " " + account.LastName);
                        contentCustomer = contentCustomer.Replace("{{Phone}}", account.PhoneNumber);
                        contentCustomer = contentCustomer.Replace("{{Email}}", account.Email);


                    }

                   
                    contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.AccountAddress.Number + " " + order.AccountAddress.Ward + " " + order.AccountAddress.District + " " + order.AccountAddress.City);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", order.OrderDate.ToString());
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", string.Format("₫{0:#,0}", thanhtien));
                    contentCustomer = contentCustomer.Replace("{{TongTien}}", string.Format("₫{0:#,0}", TongTien));
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    //electronics_shop.Common.Common.SendMail("BenzikShop", "Đơn hàng #" + order.OrderCode, contentCustomer.ToString(), req.Email);

                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                    contentAdmin = contentAdmin.Replace("{{MaDon}}", order.OrderCode.ToString());
                    contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                    if (Session["info"] != null)
                    {
                        int userID = (int)Session["UserID"];
                        var account = db.Accounts.FirstOrDefault(a => a.AccountCode == userID);
                        contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", account.FirstName + " " + account.LastName);
                        contentAdmin = contentAdmin.Replace("{{Phone}}", account.PhoneNumber);
                        contentAdmin = contentAdmin.Replace("{{Email}}", account.Email);

                    }


                    contentAdmin = contentAdmin.Replace("{{SoNha+Phuong}}", order.AccountAddress.Number + " " + order.AccountAddress.Ward);
                    contentAdmin = contentAdmin.Replace("{{Tinh}}", order.AccountAddress.District);
                    contentAdmin = contentAdmin.Replace("{{ThanhPho}}", order.AccountAddress.City);
                    contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.AccountAddress.Number);
                    contentAdmin = contentAdmin.Replace("{{NgayDat}}", order.OrderDate.ToString());
                    contentAdmin = contentAdmin.Replace("{{ThanhTien}}", string.Format("₫{0:#,0}", thanhtien));
                    contentAdmin = contentAdmin.Replace("{{TongTien}}", string.Format("₫{0:#,0}", TongTien));
                    contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                    //electronics_shop.Common.Common.SendMail("BenzikShop", "Đơn hàng mới #" + order.OrderCode, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                    //code = new { Success = true, Code = 1 };
                    cart.ClearCart();
                    return RedirectToAction("CheckOutSuccess");
                }
            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult AddToCart(string id, int quantity)
        {
            var code = new { Success = false, msg="", code = -1,Count = 0 };
            var erAc = new { Success = true, msg = "" };
            var erQuan = new { Success = true, msg = "" };
            ECOMMERCE5Entities db = new ECOMMERCE5Entities();
            var checkProduct = db.Products.FirstOrDefault(x => x.ProductCode == id);
            if (quantity > checkProduct.Quantity || quantity == 0)
            {
                erQuan = new { Success = false, msg = "Số lượng mua phải khác 0 và nhỏ số lượng hiện có" };
                return Json(erQuan);
            }
            else
            {
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
                        code = new { Success = true, msg = "Thêm sản phầm thành công" ,code = 1, Count = cart.Items.Count };
                    }
                    return Json(code);
                }
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