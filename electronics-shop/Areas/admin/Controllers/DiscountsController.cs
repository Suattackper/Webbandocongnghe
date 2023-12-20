using electronics_shop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class DiscountsController : Controller
    {
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        // GET: Admin/Discounts
        public ActionResult Index(int page = 1, int pagesize = 9)
        {
            List<Promotion> data = db.Promotions.ToList();
            ViewBag.Promotion = data;
            return View(data.ToPagedList(page, pagesize));
        }
        [HttpPost]
        public ActionResult Create(string code, string percen, string enddate, string quantity, string startdate, int page = 1, int pagesize = 9)
        {
            Promotion p = new Promotion();
            p.PromotionCode = code;
            p.PromotionPercentage = int.Parse(percen);
            p.EndDate = DateTime.Parse(enddate);
            p.StartDate = DateTime.Parse(startdate);
            p.Quantity = int.Parse(quantity);
            db.Promotions.Add(p);
            db.SaveChanges();
            List<Promotion> data = db.Promotions.ToList();
            ViewBag.Promotion = data;
            return View("Index", data.ToPagedList(page, pagesize));
        }
        //public ActionResult Edit(string id)
        //{
        //    int code = int.Parse(id);
        //    Brand brand = db.Brands.FirstOrDefault(p => p.BrandCode == code);
        //    return View(brand);
        //}
        //[HttpPost]
        //public ActionResult Edit(string id, string name, string origin)
        //{
        //    int code = int.Parse(id);
        //    Brand brand = db.Brands.FirstOrDefault(p => p.BrandCode == code);
        //    if (name != null && name != "") brand.BrandName = name;
        //    if (origin != null && origin != "") brand.Origin = origin;
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        //public ActionResult Delete(string id)
        //{
        //    try
        //    {
        //        int code = int.Parse(id);
        //        // Kiểm tra xem có sản phẩm nào liên quan đến thương hiệu không
        //        if (db.Products.Any(p => p.BrandCode == code))
        //        {
        //            // Nếu có sản phẩm liên quan, trả về Json chứa thông báo lỗi
        //            return Json(new { success = false, message = "Không thể xóa thương hiệu vì có sản phẩm liên quan." });
        //        }
        //        // Nếu không có sản phẩm liên quan, tiến hành xóa thương hiệu
        //        Brand brand = db.Brands.FirstOrDefault(p => p.BrandCode == code);
        //        if (brand == null)
        //        {
        //            // Trường hợp không tìm thấy thương hiệu, trả về Json chứa thông báo lỗi
        //            return Json(new { success = false, message = "Không tìm thấy thương hiệu để xóa." });
        //        }
        //        db.Brands.Remove(brand);
        //        db.SaveChanges();
        //        // Trả về Json chứa thông báo thành công (nếu cần)
        //        return Json(new { success = true, message = "Thương hiệu đã được xóa thành công." });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý lỗi và ghi log nếu cần
        //        return Json(new { success = false, message = $"Lỗi xóa thương hiệu: {ex.Message}" });
        //    }
        //    //int code = int.Parse(id);
        //    //Brand brand = db.Brands.FirstOrDefault(p => p.BrandCode == code);
        //    //db.Brands.Remove(brand);
        //    //db.SaveChanges();
        //    //return RedirectToAction("Index");
        //}
    }
}