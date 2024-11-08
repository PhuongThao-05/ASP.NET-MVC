using DAWEBJollybee.Models.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        // GET: Admin/Home
        public ActionResult Index()
        {
            int selectedmonth = DateTime.Now.Month;
            if (Session["Month"] != null)
            {     
            selectedmonth = (int)Session["Month"];
            }
            int year = DateTime.Now.Year;
            var totalQuantity = (from chiTietDonHang in db.CHITIETDONHANG
                                 join donHang in db.DONHANG on chiTietDonHang.IDDH equals donHang.IDDH
                                 where donHang.NGAYDAT.Value.Month == selectedmonth && donHang.NGAYDAT.Value.Year == year && donHang.HOANTAT == true
                                 select chiTietDonHang.SOLUONG).Sum(); ;
            var completedOrdersCount = db.DONHANG.Where(m => m.NGAYDAT.HasValue && m.NGAYDAT.Value.Month == selectedmonth && m.NGAYDAT.Value.Year==year && m.HOANTAT==true).Count();
            var pendingOrdersCount = db.DONHANG.Where(m => m.NGAYDAT.HasValue && m.NGAYDAT.Value.Month == selectedmonth && m.NGAYDAT.Value.Year == year && m.HOANTAT == false && m.HUY == false).Count(); ;
            var shippingOrdersCount = db.DONHANG.Where(m => m.NGAYDAT.HasValue && m.NGAYDAT.Value.Month == selectedmonth && m.NGAYDAT.Value.Year == year && m.LastestState == "Đang giao hàng").Count();
            var sumOrder = db.DONHANG.Where(m => m.NGAYDAT.HasValue && m.NGAYDAT.Value.Month == selectedmonth && m.NGAYDAT.Value.Year == year && m.HUY==false).Count();
            var totalOrder = db.DONHANG.Where(m => m.NGAYDAT.HasValue && m.NGAYDAT.Value.Month == selectedmonth && m.NGAYDAT.Value.Year == year && m.HOANTAT == true).Sum(m => m.THANHTIEN);

            ViewBag.Sale = totalQuantity;
            ViewBag.Completed = completedOrdersCount;
            ViewBag.Pending = pendingOrdersCount;
            ViewBag.Shipping = shippingOrdersCount;
            ViewBag.SumOrder = sumOrder;
            ViewBag.Totalorder = totalOrder;
            ViewBag.SelectedMonth = selectedmonth;
            var topProducts = (from sp in db.SANPHAM
                               join ctdh in db.CHITIETDONHANG on sp.IDSP equals ctdh.IDSP
                               join dh in db.DONHANG on ctdh.IDDH equals dh.IDDH
                               where dh.HOANTAT == true
                               group new { sp, ctdh } by sp.IDSP into g
                               orderby g.Sum(x => x.ctdh.SOLUONG) descending
                               select new TopProduct
                               {
                                   Product = g.FirstOrDefault().sp, // Lấy thông tin sản phẩm đầu tiên trong nhóm
                                   TotalQuantity = (int)g.Sum(x => x.ctdh.SOLUONG)
                               }).OrderByDescending(p => p.TotalQuantity);
            var order=db.DONHANG.Take(5).ToList();
            ViewBag.Order = order;
            ViewBag.TopSP = topProducts.Take(6).ToList();
            ViewBag.Top5SP = topProducts.Take(5).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult SelectedMonth(int? selectedmonth)
        {
            Session["Month"] = selectedmonth;
            return Json(new { success = true });
        }
        }
}