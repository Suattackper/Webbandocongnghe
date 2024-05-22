using electronics_shop.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using VNPAY_CS_ASPX;

namespace electronics_shop.Controllers
{
    public class ShoppingCartController : Controller
    {

        private ECOMMERCEEntities db = new ECOMMERCEEntities();
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
            if (cart != null)
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
                return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
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
            if (check != null)
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
            OrderViewModel item = new OrderViewModel();

            ViewBag.TypePayment = TempData["type"];

            return View();
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

        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.Orders.FirstOrDefault(x => x.OrderCode == orderId);
                        if (itemOrder != null)
                        {
                            itemOrder.PaymentCode = 2;
                            db.Orders.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        ShoppingCart cart = (ShoppingCart)Session["Cart"];
                        cart.ClearCart();
                        //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                        ViewBag.Id = "Mã giao dịch thanh toán:" + orderId.ToString();
                        ViewBag.MaGiaoDich = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                        ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                        ViewBag.NganHang = "ngân hàng thanh toán:" + bankCode;
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                }
            }
            return View();
        }

        public ActionResult Patial_CheckOut(decimal total)
        {
            if (Session["info"] != null)
            {
                int userID = (int)Session["UserId"];
                var account = db.Accounts.FirstOrDefault(a => a.AccountCode == userID);

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
            var code = new { Success = false, Code = -1, Url = "" };


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
                        Promotion c = db.Promotions.FirstOrDefault(p => p.PromotionCode == magiamgia);
                        c.Quantity--;
                        db.SaveChanges();
                    }
                    order.DeliveryCode = "GHN";

                    //--------------------------------------------------------------------------//
                    int userID1 = (int)Session["UserID"];
                    var account1 = db.Accounts.FirstOrDefault(a => a.AccountCode == userID1);
                    order.AccountAddress.AccountCode = account1.AccountCode;
                    order.AccountAddress.FullName = account1.FirstName + account1.LastName;
                    //------------------------------------------------------------------------//

                    order.AccountAddress.Number = req.Number;
                    order.AccountAddress.District = req.District;
                    order.AccountAddress.Ward = req.Ward;
                    order.AccountAddress.City = req.City;
                    TempData["type"] = req.TypePayment;
                    order.AccountCode = (int)Session["UserId"]; // chỗ này nhập accountcode đã có trong csdl của m, hoặc truyên fvaof khi đăng nhập thành công
                    order.AccountAddressCode = order.AccountAddressCode;  // chỗ này tạo đại 1 accountaddress trong csdl r nhập accountaddresscode vào
                    order.PaymentCode = 1;
                    // chỗ này tạo đại 1 payment trong csdl r nhập PaymentCode vào
                    order.OrderDate = DateTime.Now;
                    order.OrderTotal = decimal.Parse(total); //(decimal?)cart.Items.Sum(x => (x.Price * x.Quantity));   // chỗ này nhập tổng bill vào
                    order.OrderNote = "";  // chỗ này nhập ghi chú vaof 
                    order.Delivered = false; //chưa vận chuyển
                    //product.Quantity = product.Quantity - cart.Items.Sum(x=>(x.Quantity));
                    //db.SaveChanges();
                    db.Orders.Add(order);
                    db.SaveChanges();
                    //lấy mã ordercode
                    Order ordercode = db.Orders.OrderByDescending(o => o.OrderCode).FirstOrDefault();

                    foreach (var i in cart.Items)
                    {
                        OrderDetail p = new OrderDetail();
                        p.ProductCode = i.ProductId;
                        p.Quantity = i.Quantity;
                        p.Price = decimal.Parse(i.Price.ToString());
                        p.OrderCode = order.OrderCode;
                        p.Total = decimal.Parse((i.Price * i.Quantity).ToString());

                        Product f = db.Products.FirstOrDefault(j => j.ProductCode == i.ProductId);
                        List<Promotion> check = db.Promotions.Where(h => h.StartDate <= DateTime.Now && h.EndDate >= DateTime.Now && h.Quantity > 0).ToList();
                        foreach (var item in check)
                        {
                            if (f.PromotionCode == item.PromotionCode)
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
                    foreach (var i in listorderdetail)
                    {
                        Product p = db.Products.FirstOrDefault(h => h.ProductCode == i.ProductCode);
                        p.Quantity = p.Quantity - i.Quantity;
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                    // send mail cho khach hang
                    var strSanPham = "";
                    var thanhtien = decimal.Zero;
                    var TongTien = decimal.Zero;
                    foreach (var sp in cart.Items)
                    {
                        strSanPham += "<tr>";
                        strSanPham += "<td>" + sp.ProductName + "</td>";
                        strSanPham += "<td>" + sp.Quantity + "</td>";
                        strSanPham += "<td>" + string.Format("₫{0:#,0}", sp.ToTalPrice) + "</td>";
                        strSanPham += "</tr>";
                        thanhtien += (decimal)(sp.Price * sp.Quantity);
                    }
                    if (mucgiam != 0)
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


                    electronics_shop.Common.Common.SendMail("Besnik.", "Đơn hàng #" + order.OrderCode, contentCustomer.ToString(), req.Email);



                    cart.ClearCart();
                    
                    code = new { Success = true, Code = req.TypePayment, Url = "" };
                    if (req.TypePayment == 2)
                    {
                        var url = UrlPayment(req.TypePaymentVN, order.OrderCode);
                        code = new { Success = true, Code = req.TypePayment, Url = url };
                    }
                }
            }
            return Json(code);
        }
        

        //thanh toan vnpay
        public string UrlPayment(int TypePaymentVN, int orderCode)
        {
            var urlPayment = "";
            var order = db.Orders.FirstOrDefault(x => x.OrderCode == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key



            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.OrderTotal * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.OrderDate.Value.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderCode);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderCode.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return urlPayment;
        }

        [HttpPost]
        public ActionResult AddToCart(string id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            var erAc = new { Success = true, msg = "" };
            var erQuan = new { Success = true, msg = "" };
            ECOMMERCEEntities db = new ECOMMERCEEntities();
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
                        code = new { Success = true, msg = "Thêm sản phầm thành công", code = 1, Count = cart.Items.Count };
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
                cart.UpdateQuantity(id, quantity);
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
                return Json(new { Success = true, Count = 0 });
            }
            return Json(new { Success = false, Count = cart.Items.Count });
        }
    }
}