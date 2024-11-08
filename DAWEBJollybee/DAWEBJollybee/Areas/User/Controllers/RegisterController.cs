using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
/*using System.Data.Entity.Validation;*/
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.User.Controllers

{
    public class RegisterController : Controller
    {
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        // GET: User/Register
        [HttpGet]
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
                    bool isUsernameExists = db.NGUOIDUNG.Any(m => m.USERNAME == obj.USERNAME);

                    if (isUsernameExists)
                    {
                        ModelState.AddModelError("USERNAME", "Tên người dùng đã tồn tại. Vui lòng nhập lại!");
                        return View(obj);
                    }
                    string username = obj.USERNAME;
                    obj.PASSWORD = HashPassword(obj.PASSWORD);
                    obj.IDENTIFY = true;
                    db.NGUOIDUNG.Add(obj);
                    db.SaveChanges();
                    Session["username"] = username;
                    return RedirectToAction("Create", "KhachHangUI");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    return View(obj);
                }
                catch
                {
                    return View(obj);
                }
            }
            return View(obj);
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
    }
}