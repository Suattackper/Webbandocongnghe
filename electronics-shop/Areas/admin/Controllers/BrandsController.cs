using electronics_shop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class BrandsController : Controller
    {
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        // GET: Admin/Brands
        public ActionResult Index(int page = 1, int pagesize = 9)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            List<Brand> data = db.Brands.ToList();
            ViewBag.Brand = data;
            return View(data.ToPagedList(page, pagesize));
        }
        [HttpGet]
        public ActionResult Search(string search, int page = 1, int pagesize = 9)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            ViewBag.Search = search;
            List<Brand> data = db.Brands.Where(p => p.BrandName.Contains(search)).ToList();
            ViewBag.Brand = data;
            return View("Index", data.ToPagedList(page, pagesize));
        }
        [HttpPost]
        public ActionResult Create(string name, string origin, int page = 1, int pagesize = 9)
        {
            Brand brand = new Brand();
            brand.BrandName = name;
            brand.Origin = origin;
            db.Brands.Add(brand);
            db.SaveChanges();
            List<Brand> data = db.Brands.ToList();
            ViewBag.Brand = data;
            return View("Index", data.ToPagedList(page, pagesize));
        }
        public ActionResult Edit(string id)
        {
            int code = int.Parse(id);
            Brand brand = db.Brands.FirstOrDefault(p => p.BrandCode == code);
            return View(brand);
        }
        [HttpPost]
        public ActionResult Edit(string id, string name, string origin)
        {
            int code = int.Parse(id);
            Brand brand = db.Brands.FirstOrDefault(p => p.BrandCode == code);
            if (name != null && name != "") brand.BrandName = name;
            if (origin != null && origin != "") brand.Origin = origin;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string id)
        {
            int code = int.Parse(id);
            // Kiểm tra xem có sản phẩm nào liên quan đến thương hiệu không
            if (db.Products.Any(p => p.BrandCode == code))
            {
                TempData["Error"] = "Error -- Không thể xóa thương hiệu vì có sản phẩm liên quan!";
                return RedirectToAction("Index");
            }
            // Nếu không có sản phẩm liên quan, tiến hành xóa thương hiệu
            Brand brand = db.Brands.FirstOrDefault(p => p.BrandCode == code);
            if (brand == null)
            {
                // Trường hợp không tìm thấy thương hiệu, trả về Json chứa thông báo lỗi
                TempData["Error"] = "Error -- Không tìm thấy thương hiệu để xóa!";
                return RedirectToAction("Index");
            }
            db.Brands.Remove(brand);
            db.SaveChanges();
            // Trả về Json chứa thông báo thành công (nếu cần)
            return RedirectToAction("Index");
        }
    }
}