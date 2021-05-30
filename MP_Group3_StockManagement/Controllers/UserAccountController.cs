using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MP_Group3_StockManagement.Models;

namespace MP_Group3_StockManagement.Controllers
{
    public class UserAccountController : Controller
    {
        MP_StockManagementContext db = new MP_StockManagementContext();
        // GET: UserAccount
        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            return View(db.UserAccounts.ToList());
        }

        [AllowAnonymous]
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(UserAccount model)
        {
            if (db.UserAccounts.Any(x => x.Username == model.Username))
            {
                ViewBag.Notification = "This account has already existed";
                return View();
            }
            else
            {
                db.UserAccounts.Add(model);
                db.SaveChanges();
                return RedirectToAction("Login", "UserAccount");
            }
           
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserAccount model, string returnUrl)
        {
            var dataItem = db.UserAccounts.Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefault();
            if (dataItem != null)
            {
                FormsAuthentication.SetAuthCookie(dataItem.Username, false);
                if(Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else {
                    Session["Username"] = model.Username.ToString();
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewData["LoginError"] = true;
                ModelState.AddModelError("LoginError", "Invalid username or password");
                return View();
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}