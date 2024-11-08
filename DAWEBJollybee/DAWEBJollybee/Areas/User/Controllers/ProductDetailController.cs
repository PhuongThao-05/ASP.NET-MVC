using DAWEBJollybee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.User.Controllers
{
    public class ProductDetailController : Controller
    {
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();

        // GET: User/ProductDetail
        public ActionResult Index(int id)
        {
            List<LOAISP> lsp = db.LOAISP.ToList();
            ViewBag.ListCategory = lsp;

            var objProduct = db.SANPHAM.Where(m => m.IDSP == id).FirstOrDefault();
            List<Models.HINHANHSP> image = db.HINHANHSP.Where(m => m.IDSP == id).ToList();
            ViewBag.Image = image;

            var similarProducts = GetSimilarProducts(objProduct.LOAISP.IDLSP); 

            ViewBag.SimilarProducts = similarProducts;
            Session["CurrentController"] = "ProductDetail";
            Session["CurrentAction"] = "Index";
            Session["idpage"] = id;
            return View(objProduct);
        }
        private List<SANPHAM> GetSimilarProducts(int categoryId)
        {
            return db.SANPHAM.Where(p => p.LOAISP.IDLSP == categoryId).Take(4).ToList();
        }
    }
}