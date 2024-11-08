using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.User.Controllers
{
    public class LoginUIController : Controller
    {
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        // GET: User/LoginUI
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Models.NGUOIDUNG obj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Models.NGUOIDUNG check = db.NGUOIDUNG.FirstOrDefault(m => m.USERNAME == obj.USERNAME && m.IDENTIFY == true);
                    if (check != null)
                    {
                        Session["user"] = obj.USERNAME;
                        string enteredPassword = HashPassword(obj.PASSWORD);
                        if (String.Equals(enteredPassword, check.PASSWORD, StringComparison.OrdinalIgnoreCase))
                        {
                            Session["Login"] = check;
                            string returnUrl = Session["ReturnUrl"] as string;
                            if (string.IsNullOrEmpty(returnUrl))
                            {
                                return RedirectToAction("Index", "HomeUI");
                            }
                            else
                            {
                                Session["ReturnUrl"] = null;
                                return Redirect(returnUrl);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng!");
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
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
        public ActionResult Logout()
        {
            string currentController = Session["CurrentController"] as string;
            string currentAction = Session["CurrentAction"] as string;
            int idpage = (int)(Session["idpage"] ?? 0);
            Session["ReturnUrl"] = Request.UrlReferrer?.ToString();
            Session["Login"] = null;
            Session["user"] = null;
            Session["count"] = null;
            if (idpage != 0 && !string.IsNullOrEmpty(currentController) && !string.IsNullOrEmpty(currentAction))
            {
                return RedirectToAction(currentAction, currentController, new { id = idpage });
            }
            else if (!string.IsNullOrEmpty(currentController) && !string.IsNullOrEmpty(currentAction))
            {
                return RedirectToAction(currentAction, currentController);
            }
            else
            {
                return RedirectToAction("Index", new { returnUrl = Request.UrlReferrer });
            }
        }

    }
}
