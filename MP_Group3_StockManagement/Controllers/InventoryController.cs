using MP_Group3_StockManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MP_Group3_StockManagement.Controllers
{
    public class InventoryController : Controller
    {
        MP_StockManagementContext db = new MP_StockManagementContext();
        // GET: Inventory
        public ActionResult Index(string search)
        {
            var inventories = from i in db.Inventories
                              where DateTime.Compare(DateTime.Now, (DateTime)i.ExpirationDate) < 1
                              select i;

            if (string.IsNullOrEmpty(search))
            {
                return View(inventories.ToList());
            }

            inventories = inventories.Where(p => p.SupplierName.Contains(search));
            return View(inventories.ToList());
        }

        public ActionResult AddInventory()
        {
            var getSupplierName = db.Suppliers.ToList();
            SelectList list = new SelectList(getSupplierName, "SupplierName", "SupplierName");
            ViewBag.listSupplier = list;

            return View();
        }

        [HttpPost]
        public JsonResult GetProducts(string id)
        {
            var getProductName = db.Products.Where(p => p.SupplierName == id).ToList();
            SelectList list = new SelectList(getProductName, "ProductName", "ProductName");

            return Json(list);
        }

        [Authorize(Roles = "Admin, user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInventory(Inventory model)
        {
            try
            {
                var log = new Log()
                {
                    Username = Session["Username"].ToString(),
                    UserAction = "Added Inventory: " + model.ProductName,
                    Date = DateTime.Now
                };

                db.Inventories.Add(model);
                db.Logs.Add(log);

                db.SaveChanges();
                return RedirectToAction("Index", "Inventory");
            }

            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.PropertyName + ": " + x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
        }

        [Authorize(Roles = "Admin, user")]
        [HttpGet]
        public ActionResult EditInventory(int id)
        {
            Inventory inventory = db.Inventories.Single(i => i.InventoryID == id);

            var getSupplierName = db.Suppliers.ToList();
            SelectList list = new SelectList(getSupplierName, "SupplierName", "SupplierName");
            ViewBag.listSupplier = list;

            var getProductName = db.Products.ToList();
            SelectList listProduct = new SelectList(getProductName, "ProductName", "ProductName");
            ViewBag.listProduct = listProduct;

            return View(inventory);
        }

        [Authorize(Roles = "Admin, user")]
        [HttpPost]
        public ActionResult EditInventory(int id, Inventory model)
        {
            try
            {
                var log = new Log()
                {
                    Username = Session["Username"].ToString(),
                    UserAction = "Edited Inventory: " + model.ProductName,
                    Date = DateTime.Now
                };

                db.Logs.Add(log);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "Inventory");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Details(int id)
        {
            Inventory inventory = db.Inventories.Single(p => p.InventoryID == id);
            return View(inventory);
        }

        [Authorize(Roles = "Admin, user")]
        [HttpPost]
        [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
        public ActionResult DeleteInventory(int id)
        {
            var inventory = db.Inventories.FirstOrDefault(p => p.InventoryID == id);
            
            var log = new Log()
            {
                Username = Session["Username"].ToString(),
                UserAction = "Deleted Inventory: " + inventory.ProductName,
                Date = DateTime.Now
            };

            db.Inventories.Remove(inventory);
            db.Logs.Add(log);
            db.Entry(inventory).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Index", "Inventory");
        }
    }
}