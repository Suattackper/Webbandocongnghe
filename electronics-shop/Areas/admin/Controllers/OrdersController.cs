using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Admin/Orders
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Trash()
        {
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }
    }
}