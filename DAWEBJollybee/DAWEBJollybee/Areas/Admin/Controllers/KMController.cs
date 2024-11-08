using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.Admin.Controllers
{
    [Authorize]
    public class KMController : Controller
    {
        // GET: Admin/KM
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        public ActionResult Index(string search, string currentFilter, int? pageSize, string sortOrder, int page = 1)
        {
           UpdateExpiredVouchers();
            var data = db.VOUCHER.Where(m => m.HSD >= DateTime.Now).ToList();
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
                data = db.VOUCHER.Where(m => m.TENVOUCHER.ToLower().Contains(search.ToLower())&& m.HSD >= DateTime.Now).ToList();
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
                    data = data.OrderBy(m => m.IDCoupon).ToList();
                    break;
                default:
                    data = data.OrderByDescending(m => m.IDCoupon).ToList();
                    break;
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NewSortOrder = sortOrder == "desc" ? "asc" : "desc";
            return View(data.ToPagedList(page, currentPageSize));
        }
            private void UpdateExpiredVouchers()
            {
            try
            {
                // Kiểm tra và cập nhật trạng thái của voucher
                var expiredVouchers = db.VOUCHER.Where(m => m.HSD < DateTime.Now && m.StateCoupon == true).ToList();
                foreach (var voucher in expiredVouchers)
                {
                    voucher.StateCoupon = false;
                }
                db.SaveChanges();

                // Display success message
                ViewBag.SuccessMessage = "Voucher states have been updated successfully.";
            }
            catch (DbEntityValidationException ex)
            {
                // Ghi log thông tin chi tiết về lỗi
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                // Handle validation errors
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Concatenate the error messages into a single string
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Display the error message
                ViewBag.ErrorMessage = "Error updating voucher states: " + fullErrorMessage;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                ViewBag.ErrorMessage = "An error occurred while updating voucher states: " + ex.Message;
            }
    }
        [HttpGet]
        public ActionResult Create()
        {
            var obj = new Models.VOUCHER();
            obj.HSD = DateTime.Now;
            return View(obj);
        }
        [HttpPost]
        public ActionResult Create(Models.VOUCHER obj)
        {
            if (ModelState.IsValid)
            {
                    if (obj.HSD < DateTime.Now)
                    {
                        ModelState.AddModelError("HSD", "Ngày hết hạn phải lớn hơn hoặc bằng ngày hiện tại.");
                        return View(obj);
                    }
                obj.StateCoupon = true;
                db.VOUCHER.Add(obj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var obj = db.VOUCHER.Find(id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit(Models.VOUCHER obj)
        {
            try
            {
                var vou = db.VOUCHER.Find(obj.IDCoupon);                
               vou.TENVOUCHER = obj.TENVOUCHER;
               vou.HSD = obj.HSD;
               vou.GIATRI = obj.GIATRI;
               vou.StateCoupon = obj.StateCoupon;
               db.SaveChanges();
            }
            catch { }           
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var obj = db.VOUCHER.Find(id);
            return View(obj);
        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            try
            {
                var macp = db.VOUCHER.Find(id);
                if (macp != null)
                {
                    db.VOUCHER.Remove(macp);
                    db.SaveChanges();
                }
            }
            catch { }
            return Json(new { success = true, redirectUrl = "/Admin/KM/Index" });
        }
    }
}