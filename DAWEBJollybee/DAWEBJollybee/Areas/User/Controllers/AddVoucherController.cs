using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.User.Controllers
{
    public class AddVoucherController : Controller
    {
        // GET: User/AddVoucher
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        public ActionResult Index()
        {
            UpdateVoucherStatus();
            Session["CurrentController"] = "AddVoucher";
            Session["CurrentAction"] = "Index";
            Session["idpage"] = 0;
            string username = Session["user"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                var khachHang = db.KHACHHANG.FirstOrDefault(m => m.USERNAME == username);
                if (khachHang != null)
                {
                    int id = khachHang.IDKH;
                    var currentDate = DateTime.Now;
                    var vouchers = (from voucher in db.VOUCHER
                                    where (voucher.StateCoupon == true &&
                                           (!db.QLVOUCHER.Any(qv => qv.IDCoupon == voucher.IDCoupon && qv.IDKH == id) ||
                                            DbFunctions.AddDays(db.QLVOUCHER.FirstOrDefault(qv => qv.IDCoupon == voucher.IDCoupon && qv.IDKH == id).NGAYTHEM, 3) >= currentDate))
                                    select voucher).ToList();
                    ViewBag.vouchers = vouchers;
                }
            }  
            else
                {
                    var vouchers = db.VOUCHER
                    .Where(v => v.StateCoupon == true)
                    .ToList();
                    ViewBag.vouchers = vouchers;
                }
            var qlv = db.QLVOUCHER.ToList();
            ViewBag.qlvoucher = qlv;
            return View();
        }
        private void UpdateVoucherStatus()
        {
            try
            {
                var expiredVouchers = db.VOUCHER.Where(m => m.HSD <= DateTime.Now).ToList();

                foreach (var voucher in expiredVouchers)
                {
                    var qlVoucher = db.QLVOUCHER.Where(q => q.StateUse == true && q.IDCoupon == voucher.IDCoupon).ToList();
                    if (qlVoucher != null)
                    {
                        foreach (var qlv in qlVoucher)
                        {
                            qlv.StateUse = false;
                            voucher.StateCoupon = false;
                        }
                    }
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
        public ActionResult CheckVoucherExists(int idvoucher)
        {
            bool existsInQLVoucher = false;
            string username = Session["user"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                var khachHang = db.KHACHHANG.FirstOrDefault(m => m.USERNAME == username);
                if (khachHang != null)
                {
                    int id = khachHang.IDKH;
                    existsInQLVoucher = db.QLVOUCHER.Any(qv => qv.IDCoupon == idvoucher && qv.IDKH == id);
                }
            }
                return Json(new { exists = existsInQLVoucher, keepStatus = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Addvoucher(int idvoucher)
        {
            string username = Session["user"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                var khachHang = db.KHACHHANG.FirstOrDefault(m => m.USERNAME == username);
                if (khachHang != null)
                {
                    int id = khachHang.IDKH;
                    if (idvoucher > 0)
                    {
                        var obj = new Models.QLVOUCHER { IDKH = id, IDCoupon = idvoucher, StateUse = true,NGAYTHEM=DateTime.Now };
                        db.QLVOUCHER.Add(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Không thể tạo trạng thái do không có ID hợp lệ"}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ManageVoucher()
        {
            UpdateVoucherStatus();
            string username = Session["user"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                var khachHang = db.KHACHHANG.FirstOrDefault(m => m.USERNAME == username);
                if (khachHang != null)
                {
                    int id = khachHang.IDKH;
                    Session["idkhachhang"] = id;
                    var voucher = db.QLVOUCHER.Where(m => m.StateUse == true && m.IDKH==id).ToList();
                    ViewBag.Managevoucher = voucher;
                }
            }
          
            return View();
        }
    }
}