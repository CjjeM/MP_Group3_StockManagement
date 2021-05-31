using MP_Group3_StockManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var inventories = from i in db.Inventories select i;

            if (string.IsNullOrEmpty(search))
            {
                return View(inventories.ToList());
            }

            inventories = inventories.Where(p => p.SupplierName.Contains(search));
            return View(inventories.ToList());
        }

        public ActionResult AddInventory()
        {
            CascadingModel cascadingModel = new CascadingModel();
            foreach (var supplier in db.Suppliers)
            {
                cascadingModel.Suppliers.Add(new SelectListItem
                {
                    Text = supplier.SupplierName,
                    Value = supplier.SupplierName
                });
            }

            return View(cascadingModel);
        }

        [HttpPost]
        public ActionResult Dropdowns(string? supplierName, string? productName)
        {
            CascadingModel cascadingModel = new CascadingModel();

            foreach(var supplier in db.Suppliers)
            {
                cascadingModel.Suppliers.Add(new SelectListItem
                {
                    Text = supplier.SupplierName,
                    Value = supplier.SupplierName
                });
            }

            if (supplierName != null)
            {
                var products = (from product in db.Products
                                where product.SupplierName == supplierName
                                select product).ToList();

                foreach(var product in products)
                {
                    cascadingModel.Products.Add(new SelectListItem
                    {
                        Text = product.ProductName,
                        Value = product.ProductName
                    });
                }
            }

            return View(cascadingModel);
        }


        [Authorize(Roles = "Admin, user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInventory(Inventory model)
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
            return RedirectToAction("Index", "Product");
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
        public ActionResult EditProduct(int id, Inventory model)
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

                return RedirectToAction("Index");
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

            return RedirectToAction("Index");
        }
    }
}