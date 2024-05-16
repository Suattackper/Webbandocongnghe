using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewProfile()
        {
            return View();
        }

        public ActionResult BackToHome()
        {
            return Redirect("~/Home/Index");
        }
    }
}