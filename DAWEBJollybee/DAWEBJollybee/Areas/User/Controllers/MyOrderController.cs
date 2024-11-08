using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.User.Controllers
{
    public class MyOrderController : Controller
    {
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();

        // GET: User/MyOrder
        public ActionResult Index()
        {
            Session["CurrentController"] = "HomeUI";
            Session["CurrentAction"] = "Index";
            Session["idpage"] = 0;
            string username = (string)Session["user"];          
            if (!string.IsNullOrEmpty(username))               
            {  
                var customer = db.KHACHHANG.FirstOrDefault(m => m.USERNAME == username);
                List<Models.DONHANG> data = db.DONHANG.Where(d => d.IDKH == customer.IDKH).OrderByDescending(m => m.IDDH).ToList();
                ViewBag.Order = data;
            }
                return View();
        }

        [HttpPost]
        public ActionResult Cancel(int orderId)
        {
            try
            {
                var obj = new Models.TRANGTHAIDH
                {
                    IDDH = orderId,
                    CurrentState = "Hủy đơn hàng",
                    StateTime = DateTime.Now
                };
                db.TRANGTHAIDH.Add(obj);
                db.SaveChanges();
                var order = db.DONHANG.FirstOrDefault(d => d.IDDH == orderId);
                if (order != null)
                {
                    order.LastestState = "Hủy đơn hàng";
                    order.HUY = true;

                    db.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Không tìm thấy đơn hàng." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        public ActionResult ViewOrderDetail(int iddh)
        {
            var order = db.DONHANG.FirstOrDefault(d => d.IDDH == iddh);
            if (order == null)
            {
                return HttpNotFound();
            }

            var orderDetails = db.CHITIETDONHANG.Where(d => d.IDDH == iddh).ToList();
            ViewBag.OrderDetails = orderDetails;

            return View(new List<Models.DONHANG> { order });
        }




    }
}
