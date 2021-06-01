using MP_Group3_StockManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MP_Group3_StockManagement.Controllers
{
    public class ReportController : Controller
    {
        MP_StockManagementContext db = new MP_StockManagementContext();
        // GET: Report
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        public ActionResult GetBelowCritical()
        {
            var model = from p in db.Products
                        where p.SafetyLevel == 1
                        select p;
            return PartialView("BelowCritical", model.ToList());
        }
        public ActionResult GetWithinCritical()
        {
            var model = from p in db.Products
                        where p.SafetyLevel == 2
                        select p;
            return PartialView("WithinCritical", model.ToList());
        }
        public ActionResult GetAboveCritical()
        {
            var model = from p in db.Products
                        where p.SafetyLevel == 3
                        select p;
            return PartialView("AboveCritical", model.ToList());
        }

        public ActionResult GetExpiredProductsList()
        {
            var model = from p in db.Products
                        where p.ExpirationDate != null &&
                        DateTime.Compare(DateTime.Now, (DateTime)p.ExpirationDate) > 0
                        select p;
            return PartialView("ExpiredProductsList", model.ToList());
        }

        public ActionResult GetCostCurrentAvailableStocks()
        {
            var model = from p in db.Inventories
                        where p.ExpirationDate == null ||
                        DateTime.Compare(DateTime.Now, p.ExpirationDate) < 1
                        select p.TotalPrice;

            ViewBag.CurrentStocksCost = Queryable.Sum(model);

            return PartialView("CostCurrentAvailableStocks");
        }
        public ActionResult GetCostExpiredStocks()
        {
            var model = from p in db.Inventories
                        where p.ExpirationDate != null &&
                        DateTime.Compare(DateTime.Now, p.ExpirationDate) > 0
                        select p.TotalPrice;

            ViewBag.CurrentExpiredCost = Queryable.Sum(model);

            return PartialView("CostExpiredStocks");
        }
    }
}