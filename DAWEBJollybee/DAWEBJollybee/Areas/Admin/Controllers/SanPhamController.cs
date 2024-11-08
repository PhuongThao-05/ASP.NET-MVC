using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace DAWEBJollybee.Areas.Admin.Controllers
{
    [Authorize]
    public class SanPhamController : Controller
    {
        // GET: Admin/SanPham
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        public ActionResult Index(string search, string currentFilter, int? pageSize, string sortOrder, int page = 1)
        {
            var data = db.SANPHAM.Where(m => m.StateProduct == true).ToList();
            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = currentFilter;
            }
            if (!string.IsNullOrEmpty(search))
            {
                data = db.SANPHAM.Where(m => m.TENSP.ToLower().Contains(search.ToLower()) && m.StateProduct == true).ToList();
            }
            int defaultPageSize = 5;
            int currentPageSize = pageSize ?? defaultPageSize;
            ViewBag.CurrentFilter = search;
            ViewBag.Pagesize = currentPageSize;
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "desc";
            }
            switch (sortOrder)
            {
                case "asc":
                    data = data.OrderBy(m => m.IDSP).ToList();
                    break;
                default:
                    data = data.OrderByDescending(m => m.IDSP).ToList();
                    break;
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NewSortOrder = sortOrder == "desc" ? "asc" : "desc";
            return View(data.ToPagedList(page, currentPageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Maloaisp = new SelectList(db.LOAISP.ToList(), "IDLSP", "TENLOAISP");
            return View();
        }
        [HttpPost]
        public ActionResult Create(Models.SANPHAM obj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var fImage = Request.Files["fImage"];
                    if (fImage != null && fImage.ContentLength > 0)
                    {
                        string originalFileName = Path.GetFileName(fImage.FileName);
                        string uniqueFileName = Path.GetFileNameWithoutExtension(originalFileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(originalFileName);
                        string folderPath = Server.MapPath("~/Assets/Uploads/");
                        string filePath = Path.Combine(folderPath, uniqueFileName);
                        fImage.SaveAs(filePath);
                        obj.HINHANH = "/Assets/Uploads/" + uniqueFileName;
                    }
                    obj.StateProduct = true;
                    db.SANPHAM.Add(obj);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch { }
            }
            ViewBag.Maloaisp = new SelectList(db.LOAISP.ToList(), "IDLSP", "TENLOAISP");
            return View(obj);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var obj = db.SANPHAM.Find(id);
            ViewBag.IDLSP = new SelectList(db.LOAISP.ToList(), "IDLSP", "TENLOAISP");
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit(Models.SANPHAM obj)
        {
            try
            {
                var masp = db.SANPHAM.Find(obj.IDSP);
                var fImage = Request.Files["fImage"];
                if (fImage != null && fImage.ContentLength > 0)
                {
                    string originalFileName = Path.GetFileName(fImage.FileName);
                    string uniqueFileName = Path.GetFileNameWithoutExtension(originalFileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(originalFileName);
                    string folderPath = Server.MapPath("~/Assets/Uploads/");
                    string filePath = Path.Combine(folderPath, uniqueFileName);
                    fImage.SaveAs(filePath);
                    masp.HINHANH = "/Assets/Uploads/" + uniqueFileName;         
                }
                masp.IDLSP = obj.IDLSP;
                masp.TENSP = obj.TENSP;
                masp.MOTA = obj.MOTA;
                masp.DONGIA = obj.DONGIA;
                db.SaveChanges();
            }
            catch { }
            ViewBag.Maloaisp = new SelectList(db.LOAISP.ToList(), "IDLSP", "TENLOAISP");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var obj = db.SANPHAM.Find(id);
            return View(obj);
        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            try
            {
                var masp = db.SANPHAM.Find(id);
                if (masp != null)
                {
                    masp.StateProduct = false;
                    db.SaveChanges();
                }
            }
            catch { }
            return Json(new { success = true, redirectUrl = "/Admin/SanPham/Index" });
        }

        [HttpGet]
        public ActionResult IndexImg(int id)
        {
            Session["Masp"] = id;
            var data = db.HINHANHSP.Where(m => m.IDSP == id).ToList();
            return View(data);
        }

        [HttpGet]
        public ActionResult CreateImg()
        {
            int id = (int)Session["Masp"];
            ViewBag.Id = id;
            var obj = new Models.HINHANHSP { IDSP = id };
            return View(obj);
        }
        [HttpPost]
        public ActionResult CreateImg(Models.HINHANHSP obj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int id = (int)Session["Masp"];
                    var fImage = Request.Files["fImage"];
                    if (fImage != null && fImage.ContentLength > 0)
                    {
                        string originalFileName = Path.GetFileName(fImage.FileName);
                        string uniqueFileName = Path.GetFileNameWithoutExtension(originalFileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(originalFileName);
                        string folderPath = Server.MapPath("~/Assets/Uploads/");
                        string filePath = Path.Combine(folderPath, uniqueFileName);
                        fImage.SaveAs(filePath);
                        obj.IMAGE = "/Assets/Uploads/" + uniqueFileName;
                    }
                    db.HINHANHSP.Add(obj);
                    db.SaveChanges();
                    return RedirectToAction("IndexImg", new { id = id });
                }
                catch { }
            }
            return View(obj);
        }

        [HttpGet]
        public ActionResult EditImg(int id)
        {
            var obj = db.HINHANHSP.Find(id);
            ViewBag.Id = Session["Masp"];
            return View(obj);
        }
        [HttpPost]
        public ActionResult EditImg(Models.HINHANHSP obj)
        {
            try
            {
                var img = db.HINHANHSP.Find(obj.IDHA);
                var fImage = Request.Files["fImage"];
                if (fImage != null && fImage.ContentLength > 0)
                {
                    string originalFileName = Path.GetFileName(fImage.FileName);
                    string uniqueFileName = Path.GetFileNameWithoutExtension(originalFileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(originalFileName);
                    string folderPath = Server.MapPath("~/Assets/Uploads/");
                    string filePath = Path.Combine(folderPath, uniqueFileName);
                    fImage.SaveAs(filePath);
                    img.IMAGE = "/Assets/Uploads/" + uniqueFileName;
                }
                img.IDSP = obj.IDSP;
                db.SaveChanges();
            }
            catch { }
            return RedirectToAction("IndexImg", new { id = Session["Masp"] });
        }

        [HttpGet]
        public ActionResult DeleteImg(int id)
        {
            var obj = db.HINHANHSP.Find(id);
            ViewBag.Id = Session["Masp"];
            return View(obj);
        }
        [HttpPost]
        [ActionName("DeleteImg")]
        public ActionResult DeleteImgConfirm(int id)
        {
            try
            {
                var masp = db.HINHANHSP.Find(id);
                if (masp != null)
                {
                    db.HINHANHSP.Remove(masp);
                    db.SaveChanges();
                }
            }
            catch { }
           /* return RedirectToAction("Index", new { id = Session["Masp"] });*/
            return Json(new { success = true, redirectUrl = "/Admin/SanPham/IndexImg/"+ Session["Masp"] });
        }

    }
}