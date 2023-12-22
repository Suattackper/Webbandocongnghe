using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class ProductsAdminController : Controller
    {
        // GET: Admin/ProductsAdmin
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

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }
    }
}