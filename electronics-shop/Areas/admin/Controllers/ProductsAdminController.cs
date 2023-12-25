using electronics_shop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class ProductsAdminController : Controller
    {
        private ECOMMERCEEntities db = new ECOMMERCEEntities();
        // GET: Admin/ProductsAdmin
        public ActionResult Index(int page = 1, int pagesize = 10)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            List<Product> data = db.Products.ToList();
            ViewBag.Brand = db.Brands.ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Promotion = db.Promotions.ToList();
            return View(data.ToPagedList(page, pagesize));
        }

        public ActionResult Trash()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Search(string search, int page = 1, int pagesize = 10)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            ViewBag.Search = search;
            List<Product> data = db.Products.Where(p => p.ProductName.Contains(search)).ToList();
            ViewBag.Brand = db.Brands.ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Promotion = db.Promotions.ToList();
            return View("Index", data.ToPagedList(page, pagesize));
        }

        public ActionResult Details()
        {
            return View();
        }
        
        public ActionResult Create()
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            ViewBag.Brand = db.Brands.ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Promotion = db.Promotions.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Create(string code, string name, string brand, string category, string price, string promotion, string quantity, string description)
        {
            HttpPostedFileBase image = Request.Files["image"];
            if (db.Products.Any(h => h.ProductCode == code))
            {
                TempData["Error"] = "Error -- Mã " + code + " đã tồn tại!";
                return RedirectToAction("Create");
            }
            if (int.Parse(price) <= 0)
            {
                TempData["Error"] = "Error -- Giá bán phải lớn hơn 0!";
                return RedirectToAction("Create");
            }
            if (int.Parse(quantity) <= 0)
            {
                TempData["Error"] = "Error -- Số lượng phải lớn hơn 0!";
                return RedirectToAction("Create");
            }
            // Kiểm tra xem có file được chọn không
            if (image != null && image.ContentLength <= 0)
            {
                TempData["Error"] = "Error -- Vui lòng chọn ảnh!";
                return RedirectToAction("Create");
            }
            Product p = new Product();
            p.ProductCode = code;
            p.ProductName = name;
            p.BrandCode = int.Parse(brand);
            p.CategoryCode = int.Parse(category);
            p.Price = decimal.Parse(price);
            if(promotion != "0") p.PromotionCode = promotion;
            p.Quantity = int.Parse(quantity);
            p.Description = description;
            p.Rate = 0;
            p.ViewCount = 0;
            // Đọc dữ liệu từ luồng dữ liệu của file
            byte[] imageData = new byte[image.ContentLength];
            image.InputStream.Read(imageData, 0, image.ContentLength);
            p.ImageProduct = imageData;
            db.Products.Add(p);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(string code)
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Error = TempData["Error"];
            }
            ViewBag.Brand = db.Brands.ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Promotion = db.Promotions.ToList();
            ViewBag.ProductImg = db.ProductImgs.ToList();
            Product p = db.Products.FirstOrDefault(h => h.ProductCode == code);
            return View(p);
        }
        [HttpPost]
        public ActionResult Edit(string code, string name, string brand, string category, string price, string promotion, string quantity, string description)
        {
            HttpPostedFileBase image = Request.Files["image"];
            if (int.Parse(price) <= 0)
            {
                TempData["Error"] = "Error -- Giá bán phải lớn hơn 0!";
                return RedirectToAction("Edit", new {code = code});
            }
            if (int.Parse(quantity) <= 0)
            {
                TempData["Error"] = "Error -- Số lượng phải lớn hơn 0!";
                return RedirectToAction("Edit", new { code = code });
            }
            Product p = db.Products.FirstOrDefault(h => h.ProductCode == code);
            p.ProductName = name;
            p.BrandCode = int.Parse(brand);
            p.CategoryCode = int.Parse(category);
            p.Price = decimal.Parse(price);
            if (promotion != "0") p.PromotionCode = promotion;
            p.Quantity = int.Parse(quantity);
            p.Description = description;
            // Kiểm tra xem có file được chọn không
            if (image != null && image.ContentLength > 0)
            {
                // Đọc dữ liệu từ luồng dữ liệu của file
                byte[] imageData = new byte[image.ContentLength];
                image.InputStream.Read(imageData, 0, image.ContentLength);
                p.ImageProduct = imageData;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult AddProductImg(string code)
        {
            HttpPostedFileBase anh = Request.Files["anh"];
            if (anh != null && anh.ContentLength > 0)
            {
                // Thực hiện xử lý hình ảnh
                ProductImg p = new ProductImg();
                p.ProductCode = code;
                byte[] imageData = new byte[anh.ContentLength];
                anh.InputStream.Read(imageData, 0, anh.ContentLength);
                p.Img = imageData;
                db.ProductImgs.Add(p);
                db.SaveChanges();
                return RedirectToAction("Edit", new { code = code });
            }
            else
            {
                // Xử lý khi không có hình ảnh được chọn
                TempData["Error"] = "Có lỗi xảy ra.";
                return RedirectToAction("Edit", new { code = code });
            }
        }
        public ActionResult Delete(string id)
        {
            // Kiểm tra xem có đơn hàng nào liên quan đến thương hiệu không
            if (db.OrderDetails.Any(p => p.ProductCode == id))
            {
                TempData["Error"] = "Error -- Không thể xóa sản phẩm vì có đơn hàng liên quan!";
                return RedirectToAction("Index");
            }
            // Nếu không có sản phẩm liên quan, tiến hành xóa thương hiệu
            Product brand = db.Products.FirstOrDefault(p => p.ProductCode == id);
            if (brand == null)
            {
                // Trường hợp không tìm thấy thương hiệu, trả về Json chứa thông báo lỗi
                TempData["Error"] = "Error -- Không tìm thấy sản phẩm để xóa!";
                return RedirectToAction("Index");
            }
            var comment = db.Comments.Where(p => p.ProductCode == id).ToList();
            if(comment.Count() > 0)
            {
                foreach (var item in comment)
                {
                    db.Comments.Remove(item);
                    db.SaveChanges();
                }
            }
            var productimg = db.ProductImgs.Where(p => p.ProductCode == id).ToList();
            if (productimg.Count() > 0)
            {
                foreach (var item in productimg)
                {
                    db.ProductImgs.Remove(item);
                    db.SaveChanges();
                }
            }
            db.Products.Remove(brand);
            db.SaveChanges();
            // Trả về Json chứa thông báo thành công (nếu cần)
            return RedirectToAction("Index");
        }

    }
}