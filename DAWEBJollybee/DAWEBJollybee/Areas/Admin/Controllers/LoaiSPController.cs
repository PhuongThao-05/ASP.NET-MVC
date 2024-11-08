using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.Admin.Controllers
{
    [Authorize]
    public class LoaiSPController : Controller
    {
        // GET: Admin/LoaiSP
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        public ActionResult Index(string search)
        {
            var list = db.LOAISP.ToList();
            if (!string.IsNullOrEmpty(search))
            {
                list = db.LOAISP.Where(m => m.TENLOAISP.ToLower().Contains(search.ToLower())).ToList();
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
        public ActionResult Create(Models.LOAISP obj)
        {
            if (ModelState.IsValid)
            {
                db.LOAISP.Add(obj);
                db.SaveChanges();
                return Json(new { success = true, redirectUrl = "/Admin/LoaiSP/Index" });
            }

            // Trả về HTML của form và lỗi
            return View(obj);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var obj = db.LOAISP.Find(id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit(Models.LOAISP obj)
        {
            try
            {
                var malsp = db.LOAISP.Find(obj.IDLSP);
                malsp.TENLOAISP = obj.TENLOAISP;
                db.SaveChanges();              
            }
            catch { }
            return Json(new { success = true, redirectUrl = "/Admin/LoaiSP/Index" });
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var obj = db.LOAISP.Find(id);
            return View(obj);
        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            try
            {
                var malsp = db.LOAISP.Find(id);
                if (malsp != null)
                {
                    db.LOAISP.Remove(malsp);
                    db.SaveChanges();
                }
            }
            catch { }
            return Json(new { success = true, redirectUrl = "/Admin/LoaiSP/Index" });
        }
    }
}