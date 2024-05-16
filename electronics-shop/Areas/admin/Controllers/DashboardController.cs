using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            DateTime now = DateTime.Now;
            int thanght = now.Month;
            int namht = now.Year;
            int thangt;
            int namt = namht;
            decimal tongdtt = 0;
            decimal tongdt = 0;
            int tongspb = 0;
            int tongsp = 0;
            if (thanght == 1)
            {
                thangt = 12;
                namt--;
            }
            else thangt = thanght - 1;

            List<Account> accountsht = db.Accounts.Where(p => p.CreateAt.Value.Month == thanght && p.CreateAt.Value.Year == namht).ToList();
            List<Account> accountstt = db.Accounts.Where(p => p.CreateAt.Value.Month == thangt && p.CreateAt.Value.Year == namt).ToList();
            if (accountsht.Count == 0 && accountstt.Count == 0)
            {
                ViewBag.Khachhangmoi = 0;
                ViewBag.Tylekhachhang = 0;
            }
            else if (accountsht.Count != 0 && accountstt.Count == 0)
            {
                ViewBag.Khachhangmoi = accountsht.Count;
                ViewBag.Tylekhachhang = accountsht.Count;
            }
            else
            {
                ViewBag.Khachhangmoi = accountsht.Count;
                ViewBag.Tylekhachhang = (accountstt.Count - accountsht.Count) / accountstt.Count * 100;
            }

            List<Account> a = db.Accounts.ToList();
            ViewBag.Khachhang = a.Count;
            List<Order> orders = db.Orders.Where(p => p.OrderDate.Value.Month == thanght && p.OrderDate.Value.Year == namht).ToList();
            foreach (Order order in orders)
            {
                tongdtt += order.OrderTotal.Value;
            }
            ViewBag.Tongdoanhthu = tongdtt;
            ViewBag.Donhangmoi = orders.Count;

            List<Order> o = db.Orders.ToList();
            foreach (Order i in o)
            {
                tongdt += i.OrderTotal.Value;
            }
            ViewBag.Doanhthu = tongdt;
            ViewBag.Donhang = o.Count;

            List<OrderDetail> sanphamban = db.OrderDetails.ToList();
            foreach (OrderDetail detail in sanphamban)
            {
                foreach (Order order in orders)
                {
                    if (detail.OrderCode == order.OrderCode) tongspb += detail.Quantity.Value;
                }
                tongsp += detail.Quantity.Value;
            }
            ViewBag.Tongspb = tongspb;
            ViewBag.Tongsp = tongsp;

            List<OrderDetail> smarthome = db.OrderDetails.ToList();
            decimal tongsmh = 0;
            foreach (OrderDetail i in smarthome)
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductCode == i.ProductCode && db.Categories.Any(h => h.CategoryCode == p.CategoryCode && h.CategoryName == "Smarthome"));
                if (product != null) tongsmh += i.Total.Value;
            }
            ViewBag.DoanhthuSmarthome = tongsmh;

            List<OrderDetail> gaminggear = db.OrderDetails.ToList();
            decimal tonggmg = 0;
            foreach (OrderDetail i in gaminggear)
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductCode == i.ProductCode && db.Categories.Any(h => h.CategoryCode == p.CategoryCode && h.CategoryName == "GamingGear"));
                if (product != null) tonggmg += i.Total.Value;
            }
            ViewBag.DoanhthuGamingGear = tonggmg;

            List<OrderDetail> accessory = db.OrderDetails.ToList();
            decimal tongacc = 0;
            foreach (OrderDetail i in accessory)
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductCode == i.ProductCode && db.Categories.Any(h => h.CategoryCode == p.CategoryCode && h.CategoryName == "Accessory"));
                if (product != null) tongacc += i.Total.Value;
            }
            ViewBag.DoanhthuAccessory = tongacc;

            List<OrderDetail> spvuaban1 = db.OrderDetails.OrderByDescending( p => p.OrderCode).ToList();
            OrderDetail spvuaban = spvuaban1.FirstOrDefault();
            if (spvuaban != null)
            {
                Product pro = db.Products.FirstOrDefault(p => p.ProductCode == spvuaban.ProductCode);
                ViewBag.Tensp = pro.ProductName;
                Order or = db.Orders.FirstOrDefault(p => p.OrderCode == spvuaban.OrderCode);
                DateTime thoigianbansp = or.OrderDate.Value;
                // Tính khoảng thời gian giữa thoigianbansp và thoiGianHienTai
                TimeSpan khoangThoiGian = now - thoigianbansp; 
                ViewBag.Khoangthoigianbansp = $"{khoangThoiGian.Days} ngày, {khoangThoiGian.Hours} giờ, {khoangThoiGian.Minutes} phút trước";
                Account ac = db.Accounts.FirstOrDefault(p => p.AccountCode == or.AccountCode);
                ViewBag.Khanhhangmuasp = ac.FirstName + " " + ac.LastName;
            }

            List<Comment> cmmoi1 = db.Comments.OrderByDescending(p => p.CommentCode).ToList();
            Comment cmmoi = cmmoi1.FirstOrDefault();
            if (cmmoi != null)
            {
                ViewBag.Sosao = cmmoi.Rate;
                Product pro = db.Products.FirstOrDefault(p => p.ProductCode == cmmoi.ProductCode);
                ViewBag.Tenspcm = pro.ProductName;
                DateTime thoigianbansp = cmmoi.CommentTime.Value;
                // Tính khoảng thời gian giữa thoigianbansp và thoiGianHienTai
                TimeSpan khoangThoiGian = now - thoigianbansp;
                ViewBag.Khoangthoigiancm = $"{khoangThoiGian.Days} ngày, {khoangThoiGian.Hours} giờ, {khoangThoiGian.Minutes} phút trước";
                Account ac = db.Accounts.FirstOrDefault(p => p.AccountCode == cmmoi.AccountCode);
                ViewBag.Khachhangcm = ac.FirstName + " " + ac.LastName;
            }

            List<Account> acc1 = db.Accounts.OrderByDescending(p => p.AccountCode).ToList();
            Account acc = acc1.FirstOrDefault();
            if (acc != null)
            {
                DateTime thoigianbansp = acc.CreateAt.Value;
                // Tính khoảng thời gian giữa thoigianbansp và thoiGianHienTai
                TimeSpan khoangThoiGian = now - thoigianbansp;
                ViewBag.Khoangthoigiankhtg = $"{khoangThoiGian.Days} ngày, {khoangThoiGian.Hours} giờ, {khoangThoiGian.Minutes} phút trước";
                ViewBag.Khachhangtg = acc.FirstName + " " + acc.LastName;
            }
            return View();
        }
    }
}