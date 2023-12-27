using electronics_shop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private ECOMMERCE5Entities db = new ECOMMERCE5Entities();
        // GET: Admin/Orders
        public ActionResult Index(int ? page)
        {
            var items = db.Orders.OrderByDescending(x => x.OrderCode).ToList();

            if(page == null)
            {
                page = 1;
            }

            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            
            return View(items.ToPagedList(pageNumber,pageSize));
        }

        public ActionResult Trash()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_SanPham(int id)
        {
            var item = db.OrderDetails.Where(x => x.OrderCode == id).ToList();
            Order o = db.Orders.FirstOrDefault(p => p.OrderCode == id);
            ViewBag.Tong = o.OrderTotal;

            return PartialView(item);
        }

        [HttpGet]
        public ActionResult Search(string search, int page = 1, int pageSize = 10)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            ViewBag.Search = search;
            List<Order> data = db.Orders.OrderBy(x => x.OrderCode).Where(x => x.Account.PhoneNumber.Contains(search)).ToList();
            ViewBag.Order = data;
            return View("Index", data.ToPagedList(page, pageSize));
        }
        public ActionResult Partial_History_Order(int id)
        {
            var item = db.Orders.Where(x => x.AccountCode == id).ToList();
            return PartialView(item);
        }

        [HttpPost]
        public ActionResult CancleOrder(int id)
        {
           
            
            var item = db.Orders.Find(id);
            if (item != null)
            {
                if (item.PaymentCode == 2 && item.Delivered == true)
                {

                    return Json(new { message = "Unsuccess", Success = false });

                }
                Order o = db.Orders.FirstOrDefault(p => p.OrderCode == id);

                if (o.PromotionCode != null)
                {
                    Promotion promo = db.Promotions.FirstOrDefault(x => x.PromotionCode == o.PromotionCode);
                    promo.Quantity++;
                    db.SaveChanges();

                }
                List<Promotion> Promo = db.Promotions.ToList();
                List<OrderDetail> odd = db.OrderDetails.Where(b => b.OrderCode == o.OrderCode).ToList();
                foreach (var i in odd)
                {
                    Product pro = db.Products.FirstOrDefault(p => p.ProductCode == i.ProductCode);
                    pro.Quantity += i.Quantity;
                    db.SaveChanges();
                    if (pro.PromotionCode != null)
                    {
                        foreach (var pr in Promo)
                        {
                            if (pro.PromotionCode == pr.PromotionCode)
                            {
                                pr.Quantity++;
                                db.SaveChanges();
                            }
                        }
                    }
                }

                db.Orders.Remove(item);
                db.SaveChanges();


                return Json(new { message = "Success", Success = true });

            }
            return Json(new { message = "Unsuccess", Success = false });

        }

        [HttpPost]
        public ActionResult UpdateTT(int id,int trangthai, bool giaohang)
        {
            var item = db.Orders.Find(id);
            if (item != null )
            {
                db.Orders.Attach(item);
                item.PaymentCode = trangthai;
                item.Delivered = giaohang;
                db.Entry(item).Property(x => x.PaymentCode).IsModified = true;
                db.Entry(item).Property(x => x.Delivered).IsModified = true;

                db.SaveChanges();
                return Json(new { message = "Success", Success = true });

            }
            return Json(new { message = "Unsuccess", Success = false });
        }
    }
}