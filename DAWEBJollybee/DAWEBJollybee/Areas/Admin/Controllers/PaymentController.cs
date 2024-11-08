using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.Admin.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        // GET: Admin/Payment
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        public ActionResult Index(string search)
        {
            var list = db.PAYMENTMETHOD.ToList();
            if (!string.IsNullOrEmpty(search))
            {
                list = db.PAYMENTMETHOD.Where(m => m.METHODNAME.ToLower().Contains(search.ToLower())).ToList();
            }
            ViewBag.CurrentFilter = search;
            return View(list);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Models.PAYMENTMETHOD obj)
        {
            if (ModelState.IsValid)
            {
                db.PAYMENTMETHOD.Add(obj);
                db.SaveChanges();
                return Json(new { success = true, redirectUrl = "/Admin/Payment/Index" });
            }

           
            return View(obj);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var obj = db.PAYMENTMETHOD.Find(id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit(Models.PAYMENTMETHOD obj)
        {
            try
            {
                var mapm = db.PAYMENTMETHOD.Find(obj.IDPAYMENT);
                mapm.METHODNAME = obj.METHODNAME;
                db.SaveChanges();
            }
            catch { }
            return Json(new { success = true, redirectUrl = "/Admin/Payment/Index" });
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var obj = db.PAYMENTMETHOD.Find(id);
            return View(obj);
        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            try
            {
                var mapm = db.PAYMENTMETHOD.Find(id);
                if (mapm != null)
                {
                    db.PAYMENTMETHOD.Remove(mapm);
                    db.SaveChanges();
                }
            }
            catch { }
            return Json(new { success = true, redirectUrl = "/Admin/Payment/Index" });
        }
    }
}