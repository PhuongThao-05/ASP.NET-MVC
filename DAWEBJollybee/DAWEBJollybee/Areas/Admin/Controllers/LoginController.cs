using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DAWEBJollybee.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        // GET: Admin/Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Models.NGUOIDUNG obj, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Models.NGUOIDUNG check = db.NGUOIDUNG.FirstOrDefault(m => m.USERNAME == obj.USERNAME && m.PASSWORD == obj.PASSWORD && m.IDENTIFY == false);
                    if (check != null)
                    {
                        FormsAuthentication.SetAuthCookie(check.USERNAME, false);
                        if (!string.IsNullOrEmpty(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index","Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng!");
                    }
                }
                catch
                {

                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("Index");
        }
    }
}