using System.Web.Mvc;
using DAWEBJollybee.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DAWEBJollybee.Areas.User.Controllers
{
    public class ProductGridController : Controller
    {
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();

        // GET: User/ProductGrid
        public ActionResult Index(int? page, string search, string filter)
        {
                Session["CurrentController"] = "ProductGrid";
                Session["CurrentAction"] = "Index";
                Session["idpage"] = 0;
                List<SANPHAM> data;

                if (!string.IsNullOrEmpty(search))
                {
                    // Nếu có từ khóa tìm kiếm, lấy danh sách sản phẩm tìm kiếm
                    data = db.SANPHAM.Where(p => p.TENSP.ToLower().Contains(search.ToLower())).ToList();
                }
                else
                {
                    // Nếu không có từ khóa tìm kiếm, lấy toàn bộ danh sách sản phẩm
                    data = db.SANPHAM.ToList();
                }
            if (string.IsNullOrEmpty(filter))
            {
                filter = "All";
            }
            else if (filter!="All")
                {
                data = db.SANPHAM.Where(p => p.LOAISP.TENLOAISP.ToLower().Contains(filter.ToLower())).ToList();
                }
            else
            {
                data = db.SANPHAM.ToList();
            }
                ViewBag.SearchKeyword = search;
                ViewBag.ListProduct = data;
               ViewBag.SelectedLSP= filter;
                List<LOAISP> lsp = db.LOAISP.ToList();
                ViewBag.ListCategory = lsp;

                int itemsPerPage = 6; // Số lượng sản phẩm trên mỗi trang
                int currentPage = page ?? 1; // Trang hiện tại, nếu không được cung cấp thì mặc định là trang đầu tiên

                // Tính toán chỉ số của sản phẩm bắt đầu và kết thúc của trang hiện tại
                int startIndex = (currentPage - 1) * itemsPerPage;

                // Lấy ra danh sách sản phẩm cho trang hiện tại
                List<SANPHAM> productsOnPage = data.Skip(startIndex).Take(itemsPerPage).ToList();

                // Tính tổng số trang
                int totalPages = (int)Math.Ceiling((double)data.Count / itemsPerPage);

                ViewBag.CurrentPage = currentPage;
                ViewBag.TotalPages = totalPages;
                ViewBag.Products = productsOnPage;
                ViewBag.SearchKeyword = search; // Truyền từ khóa tìm kiếm đến view để hiển thị lại trong thanh tìm kiếm
                return View();
            }

        }
    }
