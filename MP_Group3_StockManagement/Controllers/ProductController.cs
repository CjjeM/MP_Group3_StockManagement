using MP_Group3_StockManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MP_Group3_StockManagement.Controllers
{
    public class ProductController : Controller
    {
        MP_StockManagementContext db = new MP_StockManagementContext();
        // GET: Product
        [HttpGet]
        public ActionResult Index(string search)
        {
            var products = from s in db.Products select s;

            if (string.IsNullOrEmpty(search))
            {
                return View(products.ToList());
            }

            products = products.Where(p => p.ProductName.Contains(search));
            return View(products.ToList());
        }

        public ActionResult AddProduct()
        {
            var getSupplierName = db.Suppliers.ToList();
            SelectList list = new SelectList(getSupplierName, "SupplierName", "SupplierName");
            ViewBag.listSupplier = list;

            return View();
        }

        [Authorize(Roles = "Admin, user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(Product model)
        {
            if (db.Products.Any(p => p.ProductName == model.ProductName))
            {
                ViewBag.Notification = "This product has already existed";
                return View();
            }
            else
            {
                var log = new Log()
                {
                    Username = Session["Username"].ToString(),
                    UserAction = "Added Product: " + model.ProductName,
                    Date = DateTime.Now
                };

                db.Products.Add(model);
                db.Logs.Add(log);

                db.SaveChanges();
                return RedirectToAction("Index", "Product");
            }
        }

        [Authorize(Roles = "Admin, user")]
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            Product product = db.Products.Single(p => p.ProductID == id);

            var getSupplierName = db.Suppliers.ToList();
            SelectList list = new SelectList(getSupplierName, "SupplierName", "SupplierName");
            ViewBag.listSupplier = list;

            return View(product);
        }

        [Authorize(Roles = "Admin, user")]
        [HttpPost]
        public ActionResult EditProduct(int id, Product model)
        {
            try
            {
                var log = new Log()
                {
                    Username = Session["Username"].ToString(),
                    UserAction = "Edited Product: " + model.ProductName,
                    Date = DateTime.Now
                };

                db.Logs.Add(log);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Details(int id)
        {
            Product product = db.Products.Single(p => p.ProductID == id);
            return View(product);
        }

        [Authorize(Roles = "Admin, user")]
        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductID == id);
            db.Products.Remove(product);

            var log = new Log()
            {
                Username = Session["Username"].ToString(),
                UserAction = "Deleted Product: " + product.ProductName,
                Date = DateTime.Now
            };
            db.Logs.Add(log);
            db.Entry(product).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult FilterProducts(string filter)
        {
            switch (filter)
            {
                case "in":
                    var inCirc = from p in db.Products
                                 where p.ExpirationDate == null ||
                                 DateTime.Compare(DateTime.Now, (DateTime)p.ExpirationDate) < 1
                                 select p;
                    return PartialView("_Filter", inCirc.ToList());
                case "notCirc":
                    var notInCirc = from p in db.Products
                                    where p.ExpirationDate != null &&
                                    DateTime.Compare(DateTime.Now, (DateTime)p.ExpirationDate) > 0
                                    select p;

                    return PartialView("_Filter", notInCirc.ToList());
                case "all":
                    return PartialView("_Filter", db.Products.ToList());
            }

            return PartialView("_Filter", db.Products.ToList());
        }
    }
}