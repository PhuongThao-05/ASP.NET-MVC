using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.User.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: User/Checkout
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();
        public string UrlPayment(Models.DONHANG dathang, int typementVn, int MADH)
        {
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key
            //Get payment input

            //Build URL for VNPAY
            Models.VnPayLibrary vnpay = new Models.VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", Models.VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (dathang.THANHTIEN).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (typementVn == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (typementVn == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (typementVn == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Models.Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + MADH);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);               
            // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            vnpay.AddRequestData("vnp_TxnRef", MADH.ToString());
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);            
            return paymentUrl;
        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult VNPAYCancel()
        {
            string previousOrderUrl = Session["PreviousOrderUrl"] as string;
            ResetOrder();
            return Redirect(previousOrderUrl);
        }
        public ActionResult VNPAY()
        {           
            return View();
        }
        public ActionResult ReturnVNPAY()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SelectedItem(List<Models.Metadata.SelectedProduct> selectedProducts, float total)
        {
            Session["SelectedProducts"] = selectedProducts;
            Session["Total"] = total;
            System.Diagnostics.Debug.WriteLine("Total:", total);
            return RedirectToAction("Create", "Checkout");
        }
        public JsonResult GetNewTotal(int? voucherId)
        {
            float newTotal = 0;
            float discount = 0;
            float tongTien = (float)Session["Total"];
            float phiShip = 20000;
            var voucher = db.VOUCHER.FirstOrDefault(m => m.IDCoupon == voucherId);
            if (voucher != null)
            {
                if (voucher.GIATRI < 1)
                {
                    discount -= (float)(voucher.GIATRI * tongTien);
                    newTotal = (float)(tongTien + phiShip + discount);
                }
                else if (voucher.GIATRI > 1)
                {
                    discount -= (float)(voucher.GIATRI);
                    newTotal = (float)(tongTien + phiShip + discount);
                }
            }
            else
            {
                newTotal = (float)(tongTien + phiShip);
            }
            Session["IdCounpon"] = voucherId;
            Session["NewTotal"] = newTotal;
            Session["Discount"] = discount;
            var result = new { NewTotal = newTotal, Discount = discount };
            return Json(result, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult Create()
        {
            Session["PreviousOrderUrl"] = Request.UrlReferrer.ToString();
            UpdateVoucherStatus();
            ResetOrder();
            var selectedProducts = Session["SelectedProducts"] as List<Models.Metadata.SelectedProduct>;
            if (selectedProducts != null && selectedProducts.Any())
            {
                List<Models.Metadata.CARTMetadata> cart = new List<Models.Metadata.CARTMetadata>();
                foreach (var cartItem in selectedProducts)
                {
                    var product = db.SANPHAM.Find(cartItem.ProductId);
                    var quantity = cartItem.Quantity;
                    var amount = cartItem.Amount;
                    cart.Add(new Models.Metadata.CARTMetadata { Product = product, Quantity = quantity, Amount = amount });
                }
                ViewBag.Cart = cart;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Bị null rồi");
            }
            var obj = new Models.DONHANG();
            string username = Session["user"] as string;
            float total = 0;
            if (Session["Total"] != null)
            {
                float.TryParse(Session["Total"].ToString(), out total);
                obj.TONGTIEN = total;
            }
            if (!string.IsNullOrEmpty(username))
            {
                var khachHang = db.KHACHHANG.FirstOrDefault(m => m.USERNAME == username);
                if (khachHang != null)
                {
                    int id = khachHang.IDKH;
                    obj.IDKH = id;
                    var vouchers = (from q in db.QLVOUCHER
                                    join v in db.VOUCHER on q.IDCoupon equals v.IDCoupon
                                    where q.StateUse == true && q.IDKH == id
                                    select new
                                    {
                                        QLVoucherId = q.IDCoupon,
                                        TENVOUCHER = v.TENVOUCHER
                                    }).ToList();
                    ViewBag.voucher = new SelectList(vouchers, "QLVoucherId", "TENVOUCHER");
                }
            }
            obj.PHISHIP = 20000;
            obj.KHUYENMAI = 0;
            obj.THANHTIEN = obj.TONGTIEN + obj.PHISHIP;
            List<Models.KHACHHANG> kh = db.KHACHHANG.Where(m => m.USERNAME == username).ToList();
            ViewBag.KH = kh;

            var paymentMethods = db.PAYMENTMETHOD.ToList();
            ViewBag.PaymentMethods = paymentMethods;
            return View(obj);
        }
        [HttpPost]
        public ActionResult Create(Models.DONHANG obj, int selectedPaymentMethod)
        {
            ResetOrder();
            if (ModelState.IsValid)
            {
                obj.HUY = false;
                obj.HOANTAT = false;
                obj.DeleteOrder = false;
                obj.NGAYDAT = DateTime.Now.Date;
                if (Session["NewTotal"] != null && Session["Discount"] != null)
                {
                    obj.THANHTIEN = (float)Session["NewTotal"];
                    float discount = (float)Session["Discount"];
                    obj.KHUYENMAI = Math.Abs(discount);
                    Session.Remove("NewTotal");
                    Session.Remove("Discount");
                }
                obj.IDPAYMENT = selectedPaymentMethod;
                db.DONHANG.Add(obj);
                db.SaveChanges();
                Session["IDDHorder"] = obj.IDDH;
                Session["IDKHorder"] = obj.IDKH;
                if (selectedPaymentMethod == 1)
                {
                    CreateBonus(selectedPaymentMethod);
                    return RedirectToAction("Success", "Checkout");
                }
                else if (selectedPaymentMethod == 2)
                {
                    var order = db.DONHANG.Find(obj.IDDH);
                    //Get Config Info
                    string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
                    string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
                    string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
                    string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

                    //Get payment input

                    //Build URL for VNPAY
                    Models.VnPayLibrary vnpay = new Models.VnPayLibrary();
                    vnpay.AddRequestData("vnp_Version", Models.VnPayLibrary.VERSION);
                    vnpay.AddRequestData("vnp_Command", "pay");
                    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                    vnpay.AddRequestData("vnp_Amount", (order.THANHTIEN*100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
                    /*if (typementVn == 1)
                    {
                        vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
                    }
                    else if (typementVn == 2)
                    {
                        vnpay.AddRequestData("vnp_BankCode", "VNBANK");
                    }
                    else if (typementVn == 3)
                    {
                        vnpay.AddRequestData("vnp_BankCode", "INTCARD");
                    }*/

                    vnpay.AddRequestData("vnp_BankCode", "VNBANK");
                    vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    vnpay.AddRequestData("vnp_CurrCode", "VND");
                    vnpay.AddRequestData("vnp_IpAddr", Models.Utils.GetIpAddress());
                    vnpay.AddRequestData("vnp_Locale", "vn");
                    vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.IDDH.ToString());
                    vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
                    vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                    vnpay.AddRequestData("vnp_TxnRef", order.IDDH.ToString());
                    // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
                    string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                    return Redirect(paymentUrl) ;
                }                            
            }
            ViewBag.voucher = new SelectList(db.VOUCHER.ToList(), "IDCoupon", "TENVOUCHER");
            return View(obj);
        }

        [HttpPost]
        public ActionResult CreateBonus(int selectedpayment)
        {
            var selectedItem = Session["SelectedProducts"] as List<Models.Metadata.SelectedProduct>;
            if (Session["IDDHorder"] != null && Session["IDKHorder"] !=null)
            {
                int id = (int)Session["IDDHorder"];
                int kh = (int)Session["IDKHorder"];

                if (selectedItem != null)
                {
                    foreach (var cartItem in selectedItem)
                    {
                        var ctdh = new Models.CHITIETDONHANG
                        {
                            IDDH = id,
                            IDSP = cartItem.ProductId,
                            SOLUONG = cartItem.Quantity,
                            GIATIEN = cartItem.Amount,
                        };
                        db.CHITIETDONHANG.Add(ctdh);

                        var existingCartItem = (List<Models.Metadata.CARTMetadata>)Session["cart"];
                        var itemToRemove = existingCartItem.FirstOrDefault(c => c.Product.IDSP == cartItem.ProductId);
                        if (itemToRemove != null)
                        {
                            existingCartItem.Remove(itemToRemove);
                            Session["count"] = Convert.ToInt32(Session["count"]) - 1;
                        }
                    }
                    db.SaveChanges();
                }

                if (Session["IdCounpon"] != null)
                {
                    int idcp = (int)Session["IdCounpon"];
                    var qlvoucher = db.QLVOUCHER.FirstOrDefault(m => m.IDCoupon == idcp && m.IDKH == kh);
                    qlvoucher.StateUse = false;
                    qlvoucher.NGAYSD = DateTime.Now;
                    db.SaveChanges();
                }
                if(selectedpayment==1)
                {
                    var order = db.DONHANG.Find(id);
                    order.LastestState = "Chờ xác nhận";
                    db.SaveChanges();

                    var state = new Models.TRANGTHAIDH
                    {
                        IDDH = id,
                        CurrentState = "Đặt hàng thành công",
                        StateTime = DateTime.Now,
                    };
                    db.TRANGTHAIDH.Add(state);
                    db.SaveChanges();
                }  
                else if(selectedpayment==2)
                {     
                var order = db.DONHANG.Find(id);
                order.LastestState = "Đang chuẩn bị đơn hàng";
                db.SaveChanges();

                var statefirst = new Models.TRANGTHAIDH
                {
                    IDDH = id,
                    CurrentState = "Đặt hàng thành công",
                    StateTime = DateTime.Now,
                };
                db.TRANGTHAIDH.Add(statefirst);
                db.SaveChanges();

                var statesecond = new Models.TRANGTHAIDH
                {
                    IDDH = id,
                    CurrentState = "Đang chuẩn bị đơn hàng",
                    StateTime = DateTime.Now,
                };
                db.TRANGTHAIDH.Add(statesecond);
                db.SaveChanges();
                }
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
                var qlVoucher = db.QLVOUCHER.Where(q =>q.IDCoupon == order.IDCoupon && q.IDKH==order.IDKH).ToList();
                if (qlVoucher != null)
                {
                    foreach (var qlv in qlVoucher)
                    {
                        qlv.StateUse = true;
                        qlv.NGAYSD = null;
                    }
                }
                db.SaveChanges();
                db.DONHANG.Remove(order);
            }
            db.SaveChanges();
        }
    }
}

    
