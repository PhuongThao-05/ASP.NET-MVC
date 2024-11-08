using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.User.Controllers

{
    public class KhachhangUIController : Controller
    {
        // GET: User/KhachhangUI
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        public ActionResult Index()
        {
            Session["CurrentController"] = "HomeUI";
            Session["CurrentAction"] = "Index";
            Session["idpage"] = 0;
            string username = (string)Session["user"];
            var data = db.KHACHHANG.Where(m => m.USERNAME == username).ToList();
            return View(data);
        }
        [HttpGet]
        public ActionResult Create()
        {
            string username = (string)Session["username"];
            var obj = new Models.KHACHHANG();
            obj.USERNAME = username;
            return View(obj);
        }
        [HttpPost]
        public ActionResult Create(Models.KHACHHANG obj)
        {
            if (ModelState.IsValid)
            {
                db.KHACHHANG.Add(obj);
                db.SaveChanges();
                return RedirectToAction("Index", "LoginUI");
            }
            return View(obj);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var obj = db.KHACHHANG.Find(id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit(Models.KHACHHANG obj)
        {
            try
            {
                var idkh = db.KHACHHANG.Find(obj.IDKH);
                idkh.HOTEN = obj.HOTEN;
                idkh.EMAIL = obj.EMAIL;
                idkh.SDT = obj.SDT;
                idkh.DIACHI = obj.DIACHI;
                db.SaveChanges();
            }
            catch { }

            return RedirectToAction("Index");
        }
    }
}