using electronics_shop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class FeedbacksController : Controller
    {
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        // GET: Admin/Feedbacks
        public ActionResult Index(int page = 1, int pagesize = 10)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            List<Comment> data = db.Comments.OrderBy(p => p.CommentCode).ToList();
            ViewBag.Account = db.Accounts.ToList();
            ViewBag.Product = db.Products.ToList();
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
            List<Comment> data = db.Comments.OrderBy(p => p.CommentCode).Where(p => p.ProductCode.Contains(search)).ToList();
            ViewBag.Account = db.Accounts.ToList();
            ViewBag.Product = db.Products.ToList();
            return View("Index", data.ToPagedList(page, pagesize));
        }
    }
}