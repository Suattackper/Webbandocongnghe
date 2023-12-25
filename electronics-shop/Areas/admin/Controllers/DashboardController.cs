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
            ViewBag.Order = db.Orders.ToList();
            return View();
        }
    }
}