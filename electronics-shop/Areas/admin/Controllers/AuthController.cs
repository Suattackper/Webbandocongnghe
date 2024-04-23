using electronics_shop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        // GET: Admin/Auth
        public ActionResult Index(int page = 1, int pagesize = 10)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            List<Account> data = db.Accounts.OrderBy(p => p.AccountCode).ToList();
            ViewBag.Role = db.Roles.ToList();
            return View(data.ToPagedList(page, pagesize));
        }
        [HttpGet]
        public ActionResult Search(string search, int page = 1, int pagesize = 10)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            ViewBag.Search = search;
            List<Account> data = db.Accounts.OrderBy(p => p.AccountCode).Where(p => p.PhoneNumber.Contains(search) || p.Email.Contains(search)).ToList();
            ViewBag.Role = db.Roles.ToList();
            return View("Index", data.ToPagedList(page, pagesize));
        }

        public ActionResult Details(int code)
        {
            int monthNow = DateTime.Now.Month;

            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            Account data = db.Accounts.FirstOrDefault(p => p.AccountCode == code);
            ViewBag.Role = db.Roles.ToList();
            ViewBag.Address = db.AccountAddresses.Where(a => a.AccountCode == code);
            List<Order> orderlist = db.Orders.Where(a => a.AccountCode == code).ToList();
            ViewBag.Order = orderlist;

            var count = db.Orders.Count(a => a.AccountCode == code);
            var count2 = db.Comments.Count(a => a.AccountCode == code);
            ViewBag.count = count;
            ViewBag.count2 = count2;
            decimal? Total = 0;
            //List<Order> orderlist = db.Orders.Where(a => a.AccountCode == code).ToList();
            foreach (var item in orderlist)
            {
                Total = Total + item.OrderTotal;
            }
            ViewBag.total = Total;
            //int targetMonth = DateTime.Now.Month; // December
            int targetMonth = DateTime.Now.Month; // December

            if (ViewBag.Order != null)
            {
                decimal? avgMonth = db.Orders
             .Where(a => a.AccountCode == code &&
                         a.OrderDate.HasValue &&
                         a.OrderDate.Value.Month == targetMonth)
             .Sum(a => a.OrderTotal);

                ViewBag.avg = avgMonth;
            }

            decimal? catesmarthome = 0;
            decimal? categaminggear = 0;
            decimal? catephukien = 0;
            decimal? other = 0;
            foreach (var item in orderlist)
            {
                List<OrderDetail> orderdetaillist = db.OrderDetails.Where(p => p.OrderCode == item.OrderCode).ToList();
                foreach (var i in orderdetaillist)
                {
                    Product product = db.Products.FirstOrDefault(p => p.ProductCode == i.ProductCode);
                    if (product.CategoryCode == 1) catesmarthome = catesmarthome + i.Total;
                    else if (product.CategoryCode == 2) catephukien = catephukien + i.Total;
                    else if (product.CategoryCode == 3) categaminggear = categaminggear + i.Total;
                    else other = other + i.Total;
                }
            }
            ViewBag.catesmarthome = catesmarthome;
            ViewBag.categaminggear = categaminggear;
            ViewBag.catephukien = catephukien;
            ViewBag.other = other;


            ViewBag.cate = db.Categories.ToList();


            return View(data);

        }
        public ActionResult Partial_ChiTietSanPham(int acCode)
        {

            List<Order> item = db.Orders.Where(p => p.AccountCode == acCode).ToList();




            return PartialView(item);


        }

        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Trash()
        {
            return View();
        }
        //Cấp quyền admin - manager
        public ActionResult EditAdmin(string code)
        {
            int id = int.Parse(code);
            Account p = db.Accounts.FirstOrDefault(h => h.AccountCode == id);
            if (p.RoleID == 1)
            {
                TempData["Error"] = $"Error -- Tài khoản có mã {code} có quyền ngang bạn!";
                return RedirectToAction("Index");
            }
            p.RoleID = 1;
            db.SaveChanges();
            ViewBag.Role = db.Roles.ToList();
            return RedirectToAction("Index");
        }
        //Cấp quyền nhân viên - employee
        public ActionResult EditEmlpoyee(string code)
        {
            int id = int.Parse(code);
            Account p = db.Accounts.FirstOrDefault(h => h.AccountCode == id);
            if (p.RoleID == 1)
            {
                TempData["Error"] = "Error -- Không đủ quyền hạn!";
                return RedirectToAction("Index");
            }
            p.RoleID = 2;
            db.SaveChanges();
            ViewBag.Role = db.Roles.ToList();
            return RedirectToAction("Index");
        }
        //Cấp quyền khách hàng - customer
        public ActionResult EditCustomer(string code)
        {
            int id = int.Parse(code);
            Account p = db.Accounts.FirstOrDefault(h => h.AccountCode == id);
            if(p.RoleID == 1)
            {
                TempData["Error"] = "Error -- Không đủ quyền hạn!";
                return RedirectToAction("Index");
            }
            p.RoleID = 3;
            db.SaveChanges();
            ViewBag.Role = db.Roles.ToList();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string code)
        {
            int id = int.Parse(code);
            // Kiểm tra xem có đơn hàng nào liên quan đến tài khoản không
            if (db.Orders.Any(p => p.AccountCode == id && p.Delivered == false))
            {
                TempData["Error"] = "Error -- Không thể xóa tài khoản vì có đơn hàng chưa hoàn thành!";
                return RedirectToAction("Index");
            }
            // Nếu không có đơn hàng chưa hoàn thành, tiến hành xóa tài khoản
            Account brand = db.Accounts.FirstOrDefault(p => p.AccountCode == id);
            if (brand == null)
            {
                // Trường hợp không tìm thấy tài khoản, trả về Json chứa thông báo lỗi
                TempData["Error"] = "Error -- Không tìm thấy tài khoản để xóa!";
                return RedirectToAction("Index");
            }
            var Orders = db.Orders.Where(p => p.AccountCode == id).ToList();
            if (Orders.Count() > 0)
            {
                foreach (var item in Orders)
                {
                    var orderderail = db.OrderDetails.Where(p => p.OrderCode == item.OrderCode).ToList();
                    if (orderderail.Count() > 0)
                    {
                        foreach (var i in orderderail)
                        {
                            db.OrderDetails.Remove(i);
                            db.SaveChanges();
                        }
                    }
                    db.Orders.Remove(item);
                    db.SaveChanges();
                }
            }
            var comment = db.Comments.Where(p => p.AccountCode == id).ToList();
            if (comment.Count() > 0)
            {
                foreach (var item in comment)
                {
                    db.Comments.Remove(item);
                    db.SaveChanges();
                }
            }
            var productimg = db.Contacts.Where(p => p.AccountCode == id).ToList();
            if (productimg.Count() > 0)
            {
                foreach (var item in productimg)
                {
                    db.Contacts.Remove(item);
                    db.SaveChanges();
                }
            }
            var AccountAddresses = db.AccountAddresses.Where(p => p.AccountCode == id).ToList();
            if (AccountAddresses.Count() > 0)
            {
                foreach (var item in AccountAddresses)
                {
                    db.AccountAddresses.Remove(item);
                    db.SaveChanges();
                }
            }
            db.Accounts.Remove(brand);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}