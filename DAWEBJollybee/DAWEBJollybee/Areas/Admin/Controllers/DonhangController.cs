using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.Admin.Controllers
{
    [Authorize]
    public class DonhangController : Controller
    {
        // GET: Admin/Donhang
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        public ActionResult Index(string filter, string search, string currentFilter, int? pageSize, int page = 1)
        {
            ResetOrder();
            ViewBag.SelectedFilter = filter;
            var data = db.DONHANG.OrderByDescending(m => m.IDDH).Where(m => m.DeleteOrder == false).ToList();
            if (filter == "ongoing")
            {
                data = data.OrderByDescending(m => m.IDDH).Where(m => m.HOANTAT == false && m.HUY == false).ToList();
            }
            else if (filter == "completed")
            {
                data = data.OrderByDescending(m => m.IDDH).Where(m => m.HOANTAT == true && m.HUY == false).ToList();
            }
            else if (filter == "canceled")
            {
                data = data.OrderByDescending(m => m.IDDH).Where(m => m.HOANTAT == false && m.HUY==true).ToList();
            }
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
                data = data.Where(m => m.KHACHHANG.HOTEN.ToLower().Contains(search.ToLower())).ToList();
            }
            int defaultPageSize = 5;
            int currentPageSize = pageSize ?? defaultPageSize;
            ViewBag.CurrentFilter = search;
            ViewBag.Pagesize = currentPageSize;
            return View(data.ToPagedList(page, currentPageSize));
        }
        private void ResetOrder()
        {
            var ordersWithNullState = db.DONHANG.Where(o => o.LastestState == null).ToList();
            foreach (var order in ordersWithNullState)
            {
                // Lấy các chi tiết đơn hàng liên quan đến đơn hàng có LastestState là null và xóa chúng
                var orderDetails = db.CHITIETDONHANG.Where(ctdh => ctdh.IDDH == order.IDDH).ToList();
                foreach (var orderDetail in orderDetails)
                {
                    db.CHITIETDONHANG.Remove(orderDetail);
                }
                db.SaveChanges();
                db.DONHANG.Remove(order);
            }
            db.SaveChanges();
        }

        [HttpGet]
        public ActionResult IndexBill(int id)
        {        
            var data = db.DONHANG.Find(id);
            return View(data);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Models.DONHANG obj)
        {
            if (ModelState.IsValid)
            {
                obj.PHISHIP = 20000;
                obj.KHUYENMAI = 0;
                obj.TONGTIEN = 0;
                obj.THANHTIEN = 0;
                obj.NGAYDAT = DateTime.Now;
                obj.HOANTAT = false;
                obj.HUY = false;
                obj.DeleteOrder = false;
                obj.LastestState = "Chờ xác nhận";
                db.DONHANG.Add(obj);
                db.SaveChanges();
                
                    var state = new Models.TRANGTHAIDH
                    {
                        IDDH = obj.IDDH,
                        CurrentState = "Đặt hàng thành công",
                        StateTime = DateTime.Now,
                    };
                    db.TRANGTHAIDH.Add(state);
                    db.SaveChanges();                
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var obj = db.DONHANG.Find(id);
            List<Models.TRANGTHAIDH> state = db.TRANGTHAIDH.Where(m => m.IDDH == id).ToList();
            ViewBag.State = state;
            List<Models.CHITIETDONHANG> dh = db.CHITIETDONHANG.Where(m => m.IDDH == id).ToList();
            ViewBag.Orders = dh;
            return View(obj);
        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            try
            {
                var IDDH = db.DONHANG.Find(id);
                if (IDDH != null)
                {
                    IDDH.DeleteOrder = true;
                    db.SaveChanges();
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Confirm(int id)
        {
            Session["IDDH"] = id;
            var data=db.DONHANG.Find(id);
            List<Models.TRANGTHAIDH> state = db.TRANGTHAIDH.Where(m => m.IDDH == id).ToList();
            ViewBag.State = state;
            List<Models.CHITIETDONHANG> dh = db.CHITIETDONHANG.Where(m=>m.IDDH==id).ToList();
            ViewBag.Orders = dh;
            return View(data);
        }
        [HttpGet]
        public ActionResult Preparing(int id)
        {
            Session["IDDH"] = id;
            var data = db.DONHANG.Find(id);
            List<Models.TRANGTHAIDH> state = db.TRANGTHAIDH.Where(m => m.IDDH == id).ToList();
            ViewBag.State = state;
            List<Models.CHITIETDONHANG> dh = db.CHITIETDONHANG.Where(m => m.IDDH == id).ToList();
            ViewBag.Orders = dh;
            return View(data);
        }
        [HttpGet]
        public ActionResult Complete(int id)
        {
            Session["IDDH"] = id;
            var data = db.DONHANG.Find(id);
            List<Models.TRANGTHAIDH> state = db.TRANGTHAIDH.Where(m => m.IDDH == id).ToList();
            ViewBag.State = state;
            List<Models.CHITIETDONHANG> dh = db.CHITIETDONHANG.Where(m => m.IDDH == id).ToList();
            ViewBag.Orders = dh;
            return View(data);
        }
        [HttpGet]
        public ActionResult IndexState(int id)
        {
            var data = db.DONHANG.Find(id);
            List<Models.TRANGTHAIDH> state = db.TRANGTHAIDH.Where(m => m.IDDH == id).ToList();
            ViewBag.State = state;
            List<Models.CHITIETDONHANG> dh = db.CHITIETDONHANG.Where(m => m.IDDH == id).ToList();
            ViewBag.Orders = dh;
            return View(data);
        }
        public ActionResult CreateState(string currentState)
        {
            int id = (int)Session["IDDH"];
            if (id > 0 && !string.IsNullOrEmpty(currentState))
            {
                var obj = new Models.TRANGTHAIDH { IDDH = id, CurrentState = currentState, StateTime = DateTime.Now };
                db.TRANGTHAIDH.Add(obj);
                db.SaveChanges();

                var dh = db.DONHANG.Find(id);
                if (dh != null)
                {
                    dh.LastestState = obj.CurrentState;
                    if(currentState == "Đã giao hàng thành công")
                    {
                        dh.HOANTAT = true;
                    }
                    else if (currentState == "Hủy đơn hàng")
                    {
                        dh.HUY = true;
                    }    
                    db.SaveChanges();
                }
                return Json(new { success = true, id = id }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Không thể tạo trạng thái do không có ID hợp lệ" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Detail(int id, string search)
        {
            Session["IDDH"]=id;
            var obj = db.CHITIETDONHANG.Where(m => m.IDDH == id).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(m => m.SANPHAM.TENSP.ToLower().Contains(search.ToLower())).ToList();
            }
            ViewBag.CurrentFilter = search;
            List<Models.DONHANG> stdh = db.DONHANG.Where(m => m.IDDH == id).ToList();
            ViewBag.Stdh= stdh;
            return View(obj);
        }
        [HttpGet]
        public ActionResult CreateDetail()
        {
            int id = (int)Session["IDDH"];
            ViewBag.Iddh = id;
            ViewBag.IDSP = new SelectList(db.SANPHAM.ToList(), "IDSP", "TENSP");
            var obj = new Models.CHITIETDONHANG { IDDH = id };
            return View(obj);
        }
        [HttpPost]
        public ActionResult CreateDetail(Models.CHITIETDONHANG obj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    obj.SANPHAM = db.SANPHAM.Find(obj.IDSP);
                    if (obj.SANPHAM != null)
                    {
                        obj.GIATIEN = obj.SOLUONG * obj.SANPHAM.DONGIA;
                        db.CHITIETDONHANG.Add(obj);
                        db.SaveChanges();
                        var tongTien = db.CHITIETDONHANG.Where(m => m.IDDH == obj.IDDH).Sum(m => m.GIATIEN);
                        var donhang = db.DONHANG.Find(obj.IDDH);
                        if (donhang != null)
                        {
                            if(tongTien!=null)
                            {    
                            donhang.TONGTIEN = tongTien;
                            donhang.THANHTIEN = donhang.PHISHIP + tongTien;
                            }
                            else
                            {
                                donhang.TONGTIEN = 0;
                                donhang.THANHTIEN = donhang.PHISHIP;
                            }
                            db.SaveChanges();
                        }
                        return RedirectToAction("Detail", new { id = Session["IDDH"] });
                    }
                }
                catch { }
            }
            ViewBag.IDSP = new SelectList(db.SANPHAM.ToList(), "IDSP", "TENSP");
            return View(obj);
        }

        [HttpGet]
        public ActionResult EditDetail(int iddh, int idsp)
        {
            var obj = db.CHITIETDONHANG.Find(iddh, idsp);
            ViewBag.Iddh = Session["IDDH"];
            ViewBag.IDSP = new SelectList(db.SANPHAM.ToList(), "IDSP", "TENSP");
            return View(obj);
        }
        [HttpPost]
        public ActionResult EditDetail(Models.CHITIETDONHANG obj)
        {
            try
            {
                var edit = db.CHITIETDONHANG.Find(obj.IDDH, obj.IDSP);
                Models.SANPHAM masp = db.SANPHAM.Find(obj.IDSP);
                edit.SOLUONG = obj.SOLUONG;
                edit.GIATIEN = edit.SOLUONG * masp.DONGIA;
                db.SaveChanges();
                var tongTien = db.CHITIETDONHANG.Where(m => m.IDDH == obj.IDDH).Sum(m => m.GIATIEN);
                var donhang = db.DONHANG.Find(obj.IDDH);
                if (donhang != null)
                {
                    if (tongTien != null)
                    {
                        donhang.TONGTIEN = tongTien;
                        donhang.THANHTIEN = donhang.PHISHIP + tongTien;
                    }
                    else
                    {
                        donhang.TONGTIEN = 0;
                        donhang.THANHTIEN = donhang.PHISHIP;
                    }
                    db.SaveChanges();
                }
            }
            catch { }
            ViewBag.IDSP = new SelectList(db.SANPHAM.ToList(), "IDSP", "TENSP");
            return RedirectToAction("Detail", new { id = Session["IDDH"] });
        }
        [HttpGet]
        public ActionResult DeleteDetail(int iddh, int idsp)
        {
            var obj = db.CHITIETDONHANG.Find(iddh, idsp);
            ViewBag.Iddh = Session["IDDH"];
            return View(obj);
        }
        [HttpPost]
        [ActionName("DeleteDetail")]
        public ActionResult DeleteDetailConfirm(int iddh, int idsp)
        {
            try
            {
                var delete = db.CHITIETDONHANG.Find(iddh, idsp);
                if (delete != null)
                {
                    db.CHITIETDONHANG.Remove(delete);
                    db.SaveChanges();
                    var tongTien = db.CHITIETDONHANG.Where(m => m.IDDH == iddh).Sum(m => m.GIATIEN);
                    var donhang = db.DONHANG.Find(iddh);
                    if (donhang != null)
                    {
                        if (tongTien != null)
                        {
                            donhang.TONGTIEN = tongTien;
                            donhang.THANHTIEN = donhang.PHISHIP + tongTien;
                        }
                        else
                        {
                            donhang.TONGTIEN = 0;
                            donhang.THANHTIEN = donhang.PHISHIP;
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch { }
            return Json(new { success = true, redirectUrl = "/Admin/Donhang/Detail/" + Session["IDDH"] });
        }
    }
}