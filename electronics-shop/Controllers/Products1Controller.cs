//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using electronics_shop.Models;

//namespace electronics_shop.Controllers
//{
//    public class Products1Controller : Controller
//    {
//        private ECOMMERCE5Entities db = new ECOMMERCE5Entities();

//        // GET: Products1
//        public ActionResult Index()
//        {
//            var products = db.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Promotion);
//            return View(products.ToList());
//        }

//        // GET: Products1/Details/5
//        public ActionResult Details(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Product product = db.Products.Find(id);
//            if (product == null)
//            {
//                return HttpNotFound();
//            }
//            return View(product);
//        }

//        // GET: Products1/Create
//        public ActionResult Create()
//        {
//            ViewBag.BrandCode = new SelectList(db.Brands, "BrandCode", "BrandName");
//            ViewBag.CategoryCode = new SelectList(db.Categories, "CategoryCode", "CategoryName");
//            ViewBag.PromotionCode = new SelectList(db.Promotions, "PromotionCode", "PromotionCode");
//            return View();
//        }

//        // POST: Products1/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "ProductCode,ProductName,BrandCode,CategoryCode,ImageProduct,Price,PromotionCode,Quantity,Description,ViewCount,Rate")] Product product)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Products.Add(product);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.BrandCode = new SelectList(db.Brands, "BrandCode", "BrandName", product.BrandCode);
//            ViewBag.CategoryCode = new SelectList(db.Categories, "CategoryCode", "CategoryName", product.CategoryCode);
//            ViewBag.PromotionCode = new SelectList(db.Promotions, "PromotionCode", "PromotionCode", product.PromotionCode);
//            return View(product);
//        }

//        // GET: Products1/Edit/5
//        public ActionResult Edit(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Product product = db.Products.Find(id);
//            if (product == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.BrandCode = new SelectList(db.Brands, "BrandCode", "BrandName", product.BrandCode);
//            ViewBag.CategoryCode = new SelectList(db.Categories, "CategoryCode", "CategoryName", product.CategoryCode);
//            ViewBag.PromotionCode = new SelectList(db.Promotions, "PromotionCode", "PromotionCode", product.PromotionCode);
//            return View(product);
//        }

//        // POST: Products1/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "ProductCode,ProductName,BrandCode,CategoryCode,ImageProduct,Price,PromotionCode,Quantity,Description,ViewCount,Rate")] Product product)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(product).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.BrandCode = new SelectList(db.Brands, "BrandCode", "BrandName", product.BrandCode);
//            ViewBag.CategoryCode = new SelectList(db.Categories, "CategoryCode", "CategoryName", product.CategoryCode);
//            ViewBag.PromotionCode = new SelectList(db.Promotions, "PromotionCode", "PromotionCode", product.PromotionCode);
//            return View(product);
//        }

//        // GET: Products1/Delete/5
//        public ActionResult Delete(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Product product = db.Products.Find(id);
//            if (product == null)
//            {
//                return HttpNotFound();
//            }
//            return View(product);
//        }

//        // POST: Products1/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(string id)
//        {
//            Product product = db.Products.Find(id);
//            db.Products.Remove(product);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
