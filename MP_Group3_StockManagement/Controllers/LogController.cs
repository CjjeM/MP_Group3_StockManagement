using MP_Group3_StockManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MP_Group3_StockManagement.Controllers
{
    public class LogController : Controller
    {
        MP_StockManagementContext db = new MP_StockManagementContext();
        // GET: Log
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult ViewLogs()
        {
            if (Session["Username"] != null)
            {
                if(User.IsInRole("Admin"))
                {
                    return RedirectToAction("ViewAdminLogs");
                }
                else
                {
                    return RedirectToAction("ViewUserLogs");
                }
            }
                
            return View("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ViewAdminLogs()
        {
            var logs = from log in db.Logs
                       orderby log.Date descending
                       select log;

            return View(logs.ToList());
        }

        [Authorize(Roles = "User")]
        public ActionResult ViewUserLogs()
        {
            var username = Session["Username"].ToString();
            var logs = from log in db.Logs
                       where log.Username == username
                       orderby log.Date descending
                       select log;

            return View(logs.ToList());
        }


        public ActionResult Details(int id)
        {
            Log log = db.Logs.Single(s => s.LogID == id);
            return View(log);
        }
    }
}