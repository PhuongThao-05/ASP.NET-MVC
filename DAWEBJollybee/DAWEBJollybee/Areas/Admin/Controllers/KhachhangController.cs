using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.Admin.Controllers
{
    [Authorize]
    public class KhachhangController : Controller
    {
        // GET: Admin/Khachhang
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        public ActionResult Index(string search, string currentFilter, int? pageSize, string sortOrder, int page = 1)
        {
            var data = db.KHACHHANG.ToList();
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
                data = data.Where(m => m.HOTEN.ToLower().Contains(search.ToLower())).ToList();
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
                    data = data.OrderBy(m => m.IDKH).ToList();
                    break;
                default:
                    data = data.OrderByDescending(m => m.IDKH).ToList();
                    break;
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NewSortOrder = sortOrder == "desc" ? "asc" : "desc";
            return View(data.ToPagedList(page, currentPageSize));
        }
    }
}