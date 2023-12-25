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
        public ActionResult Index(int page = 1, int pagesize = 10)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            List<Promotion> data = db.Promotions.ToList();
            ViewBag.Promotion = data;
            return View(data.ToPagedList(page, pagesize));
        }
        [HttpPost]
        public ActionResult Create(string code, string percen, string enddate, string quantity, string startdate)
        {
            if(db.Promotions.Any(h => h.PromotionCode == code))
            {
                TempData["Error"] = "Error -- Mã " + code + " đã tồn tại!";
                return RedirectToAction("Index");
            }
            if (int.Parse(percen) <= 0)
            {
                TempData["Error"] = "Error -- Mức giảm phải lớn hơn 0!";
                return RedirectToAction("Index");
            }
            if (int.Parse(quantity) <= 0)
            {
                TempData["Error"] = "Error -- Số lượng phải lớn hơn 0!";
                return RedirectToAction("Index");
            }
            if (startdate == "" || startdate == null || enddate == "" || enddate == null)
            {
                TempData["Error"] = "Error -- Vui lòng nhập đầy đủ ngày bắt đầu và ngày kết thúc!";
                return RedirectToAction("Index");
            }
            if(DateTime.Parse(enddate) < DateTime.Parse(startdate))
            {
                TempData["Error"] = "Error -- Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc!";
                return RedirectToAction("Index");
            }
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
            return RedirectToAction("Index");
        }
        public ActionResult Edit(string code)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            Promotion brand = db.Promotions.FirstOrDefault(p => p.PromotionCode == code);
            return View(brand);
        }
        [HttpPost]
        public ActionResult Edit(string code, string percen, string enddate, string quantity, string startdate)
        {
            if (int.Parse(percen) <= 0)
            {
                TempData["Error"] = "Error -- Mức giảm phải lớn hơn 0!";
                return RedirectToAction("Edit", new { code = code });
            }
            if (int.Parse(quantity) <= 0)
            {
                TempData["Error"] = "Error -- Số lượng phải lớn hơn 0!";
                return RedirectToAction("Edit", new { code = code });
            }
            if (startdate == "" || startdate == null || enddate == "" || enddate == null)
            {
                TempData["Error"] = "Error -- Vui lòng nhập đầy đủ ngày bắt đầu và ngày kết thúc!";
                return RedirectToAction("Edit", new { code = code});
            }
            if (DateTime.Parse(enddate) < DateTime.Parse(startdate))
            {
                TempData["Error"] = "Error -- Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc!";
                return RedirectToAction("Edit", new { code = code });
            }
            Promotion brand = db.Promotions.FirstOrDefault(p => p.PromotionCode == code);
            if (percen != null && percen != "") brand.PromotionPercentage = int.Parse(percen);
            if (quantity != null && quantity != "") brand.Quantity = int.Parse(quantity);
            if (startdate != null && startdate != "") brand.StartDate = DateTime.Parse(startdate);
            if (enddate != null && enddate != "") brand.EndDate = DateTime.Parse(enddate);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string code)
        {
            // Kiểm tra xem có sản phẩm nào liên quan đến mã giảm giá không
            if (db.Products.Any(p => p.PromotionCode == code))
            {
                TempData["Error"] = "Error -- Không thể xóa mã giảm giá vì có sản phẩm liên quan!";
                return RedirectToAction("Index");
            }
            // Kiểm tra xem có sản phẩm nào liên quan đến thương hiệu không
            if (db.Orders.Any(p => p.PromotionCode == code))
            {
                // Nếu có sản phẩm liên quan, trả về Json chứa thông báo lỗi
                TempData["Error"] = "Error -- Không thể xóa mã giảm giá vì có đơn hàng liên quan!";
                return RedirectToAction("Index");
            }
            // Nếu không có sản phẩm liên quan, tiến hành xóa thương hiệu
            Promotion brand = db.Promotions.FirstOrDefault(p => p.PromotionCode == code);
            if (brand == null)
            {
                // Trường hợp không tìm thấy thương hiệu, trả về Json chứa thông báo lỗi
                TempData["Error"] = "Error -- Không tìm thấy mã giảm giá để xóa!";
                return RedirectToAction("Index");
            }
            db.Promotions.Remove(brand);
            db.SaveChanges();
            // Trả về Json chứa thông báo thành công (nếu cần)
            return RedirectToAction("Index");
        }
    }
}