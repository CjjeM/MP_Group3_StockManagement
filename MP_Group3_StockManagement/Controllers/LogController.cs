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
            return View(db.Logs.ToList());
        }

        [Authorize(Roles = "User")]
        public ActionResult ViewUserLogs()
        {
            string currentUser = Session["Username"].ToString();
            return View(db.Logs.Where(u => u.Username == currentUser).ToList());
        }
    }
}