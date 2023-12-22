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
            var items = db.Orders.OrderByDescending(x => x.OrderDate).ToList();

            if(page == null)
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 15; 

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
            var item = db.OrderDetails.FirstOrDefault(x => x.OrderCode == id);
            return PartialView(item);
        }
    }
}