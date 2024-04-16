using electronics_shop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class GenresController : Controller
    {
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        public ActionResult Index(int page = 1, int pagesize = 10)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            List<Category> data = db.Categories.OrderBy(p => p.CategoryCode).ToList();
            ViewBag.Brand = data;
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
            List<Category> data = db.Categories.OrderBy(p => p.CategoryCode).Where(p => p.CategoryName.Contains(search)).ToList();
            ViewBag.Category = data;
            return View("Index", data.ToPagedList(page, pagesize));
        }
        [HttpPost]
        public ActionResult Create(string name)
        {
            Category category = new Category();
            category.CategoryName = name;
            db.Categories.Add(category);
            db.SaveChanges();
            List<Category> data = db.Categories.ToList();
            ViewBag.Brand = data;
            return RedirectToAction("Index");
        }
        public ActionResult Edit(string id)
        {
            int code = int.Parse(id);
            Category category = db.Categories.FirstOrDefault(p => p.CategoryCode == code);
            return View(category);
        }
        [HttpPost]
        public ActionResult Edit(string id, string name)
        {
            int code = int.Parse(id);
            Category category = db.Categories.FirstOrDefault(p => p.CategoryCode == code);
            if (name != null && name != "") category.CategoryName = name;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string id)
        {
            int code = int.Parse(id);
            // Kiểm tra xem có sản phẩm nào liên quan đến thương hiệu không
            if (db.Products.Any(p => p.CategoryCode == code))
            {
                TempData["Error"] = "Error -- Không thể xóa danh mục vì có sản phẩm liên quan!";
                return RedirectToAction("Index");
            }
            // Nếu không có sản phẩm liên quan, tiến hành xóa thương hiệu
            Category category = db.Categories.FirstOrDefault(p => p.CategoryCode == code);
            if (category == null)
            {
                // Trường hợp không tìm thấy thương hiệu, trả về Json chứa thông báo lỗi
                TempData["Error"] = "Error -- Không tìm thấy danh mục để xóa!";
                return RedirectToAction("Index");
            }
            db.Categories.Remove(category);
            db.SaveChanges();
            // Trả về Json chứa thông báo thành công (nếu cần)
            return RedirectToAction("Index");
        }
    }
}