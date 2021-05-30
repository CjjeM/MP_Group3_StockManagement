using MP_Group3_StockManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MP_Group3_StockManagement.Controllers
{
    public class SupplierController : Controller
    {
        MP_StockManagementContext db = new MP_StockManagementContext();
        // GET: Supplier
        public ActionResult Index(string search)
        {
            if (!String.IsNullOrEmpty(search))
            {
                var suppliers = from s in db.Suppliers select s;
                suppliers = suppliers.Where(s => s.SupplierName.Contains(search));
                return View(suppliers.ToList());
            }

            return View(db.Suppliers.ToList());
        }

        public ActionResult AddSupplier()
        {
            return View();
        }

        [Authorize(Roles = "Admin, user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSupplier(Supplier model)
        {
            if (db.Suppliers.Any(x => x.SupplierName == model.SupplierName))
            {
                ViewBag.Notification = "This supplier has already existed";
                return View();
            }
            else
            {
                var log = new Log() {
                    Username = Session["Username"].ToString(),
                    UserAction = "Added Supplier: " + model.SupplierName,
                    Date = DateTime.Now
                };

                db.Suppliers.Add(model);
                db.Logs.Add(log);

                db.SaveChanges();
                return RedirectToAction("Index", "Supplier");
            }
        }

        [Authorize(Roles = "Admin, user")]
        [HttpGet]
        public ActionResult EditSupplier(int id)
        {
            Supplier supplier = db.Suppliers.Single(s => s.SupplierID == id);
            return View(supplier);
        }

        [Authorize(Roles = "Admin, user")]
        [HttpPost]
        public ActionResult EditSupplier(int id, Supplier model)
        {
            try
            {
                var log = new Log()
                {
                    Username = Session["Username"].ToString(),
                    UserAction = "Edited Supplier: " + model.SupplierName,
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

        public ActionResult Details (int id)
        {
            Supplier supplier = db.Suppliers.Single(s => s.SupplierID == id);
            return View(supplier);
        }

        [Authorize(Roles = "Admin, user")]
        [HttpPost]
        public ActionResult DeleteSupplier(int id)
        {
            var supplier = db.Suppliers.FirstOrDefault(s => s.SupplierID == id);
            db.Suppliers.Remove(supplier);

            var log = new Log()
            {
                Username = Session["Username"].ToString(),
                UserAction = "Deleted Supplier: " + supplier.SupplierName,
                Date = DateTime.Now
            };
            db.Logs.Add(log);
            db.Entry(supplier).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        
    }
}