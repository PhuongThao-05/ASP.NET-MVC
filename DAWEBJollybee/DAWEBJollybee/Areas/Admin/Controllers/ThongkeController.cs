using DAWEBJollybee.Models.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.Admin.Controllers
{
    [Authorize]
    public class ThongkeController : Controller
    {
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        // GET: Admin/Thongke
        [HttpGet]
        public ActionResult Index(int? selectedYear)
        {
            var distinctYears =db.DONHANG.Where(m => m.NGAYDAT.HasValue).Select(m => m.NGAYDAT.Value.Year).Distinct().OrderByDescending(year => year).ToList();
            if (!selectedYear.HasValue && distinctYears.Any())
            {
                selectedYear = distinctYears.First();
            }
            var ordersByMonth = db.DONHANG.Where(m => m.NGAYDAT.HasValue && m.NGAYDAT.Value.Year == selectedYear && m.HOANTAT == true)
            .GroupBy(m => new { Year = m.NGAYDAT.Value.Year, Month = m.NGAYDAT.Value.Month })
            .Select(g => new
            {
                Month = g.Key.Year + "-" + (g.Key.Month < 10 ? "0" : "") + g.Key.Month,
                TotalRevenue = g.Sum(m => m.THANHTIEN ?? 0)
            }).OrderBy(g => g.Month).ToList();
            var result = from sanpham in db.SANPHAM
                         join ctdh in db.CHITIETDONHANG on sanpham.IDSP equals ctdh.IDSP
                         join donhang in db.DONHANG on ctdh.IDDH equals donhang.IDDH
                         where donhang.NGAYDAT.HasValue && donhang.NGAYDAT.Value.Year == selectedYear && donhang.HOANTAT == true
                         group new { sanpham, ctdh, donhang } by new { sanpham, ctdh, donhang } into g
                         select new TKProductCount
                         {
                             product = g.Key.sanpham,
                             ctdh = g.Key.ctdh,
                             dh = g.Key.donhang,
                             Quantity = (int)g.Sum(x => x.ctdh.SOLUONG)
                         };
            int currentYear = DateTime.Now.Year;
            var ordersByYear = db.DONHANG.Where(m => m.NGAYDAT.HasValue && m.HOANTAT == true &&
                m.NGAYDAT.Value.Year >= currentYear - 4 && m.NGAYDAT.Value.Year <= currentYear)
                           .GroupBy(m => m.NGAYDAT.Value.Year)
                           .Select(g => new
                           {
                               Year = g.Key,
                               TotalRevenue = g.Sum(m => m.THANHTIEN ?? 0)
                           }).OrderBy(g => g.Year).ToList();
            var chartData = new
            {
                monthData= new
                {
                    labels = ordersByMonth.Select(g => g.Month),
                    datasets = new[]
                    {
                new { label = "Total Revenue By Month", data = ordersByMonth.Select(g => g.TotalRevenue), borderWidth = 1 }                 
                    }
                },
                yearData = new
                {
                    labels = ordersByYear.Select(g => g.Year),
                    datasets = new[]
                {
                new { label = "Total Revenue By Year", data = ordersByYear.Select(g => g.TotalRevenue), borderWidth = 1 }
                 }
                },
            };
            ViewBag.Years = new SelectList(distinctYears, selectedYear);
            ViewBag.Selectedyear = selectedYear;
            ViewBag.ProductCounts = result.ToList();
            return View(chartData);
        }
        [HttpPost]
        public ActionResult Index(int selectedYear)
        {
            return RedirectToAction("Index", new { selectedYear = selectedYear });
        }
    }
}