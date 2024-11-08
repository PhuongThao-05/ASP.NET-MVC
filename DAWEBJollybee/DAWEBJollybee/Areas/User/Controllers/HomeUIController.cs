using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAWEBJollybee.Models;
namespace DAWEBJollybee.Areas.User.Controllers
{
    public class HomeUIController : Controller
    {
        // GET: User/HomeUI
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();

        // GET: User/HomeUI

        public ActionResult Index()
        {
            List<Models.SANPHAM> data = db.SANPHAM.Take(12).ToList();
            ViewBag.HotProducts = data;
            Session["CurrentController"] = "HomeUI";
            Session["CurrentAction"] = "Index";
            Session["idpage"] = 0;
            var query = (from ct in db.CHITIETDONHANG
                                       join sp in db.SANPHAM on ct.IDSP equals sp.IDSP
                                       group new { ct, sp } by new { ct.IDSP, sp.TENSP, sp.HINHANH, sp.DONGIA } into g
                                       select new
                                       {
                                           HINHANH = g.Key.HINHANH,
                                           ProductID = g.Key.IDSP,
                                           ProductName = g.Key.TENSP,
                                           DONGIA = g.Key.DONGIA,
                                           TotalQuantity = g.Sum(x => x.ct.SOLUONG)
                                       })
                                       .OrderByDescending(x => x.TotalQuantity)
                                       .Take(6)
                                       .ToList();
            List<BestSellerViewModel> list = query.ToList().Select(item => new BestSellerViewModel
            {
                HINHANH = item.HINHANH,
                ProductID = item.ProductID,
                ProductName = item.ProductName,
                DONGIA = (double)item.DONGIA,
                TotalQuantity = item.TotalQuantity??0,
            }).ToList();

            ViewBag.MostOrderedProducts = list;
            return View();
        }
       
       
    }
}