using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAWEBJollybee.Models.Metadata;

namespace DAWEBJollybee.Areas.User.Controllers
{
    public class CartController : Controller
    {
        Models.JOLLYBEEWEBEntities db = new Models.JOLLYBEEWEBEntities();

        // GET: User/Cart
        public ActionResult Index()
        {
            if (Session["Login"] == null)
            {
                return RedirectToAction("Index", "LoginUI");
            }
            else
            {
                return View((List<CARTMetadata>)Session["cart"]);
            }
        }

        public ActionResult AddToCart(int id, int quantity)
        {
            if (Session["Login"] == null)
            {
                return Json(new { success = false, totalCount = 0 , returnUrl= "/User/LoginUI/Index" });
            }
            else { 
            if (Session["cart"] == null)
            {
                List<CARTMetadata> cart = new List<CARTMetadata>();
                var product = db.SANPHAM.Find(id);
                double amount = (quantity * product.DONGIA) ?? 0;
                cart.Add(new CARTMetadata { Product = product, Quantity = quantity, Amount = amount });
                Session["cart"] = cart;
                Session["count"] = 1; // Chỉ tăng count khi thêm sản phẩm mới
                return Json(new { success = true, totalCount = 1 });
            }
            else
            {
                List<CARTMetadata> cart = (List<CARTMetadata>)Session["cart"];
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity += quantity;
                    cart[index].Amount += (quantity * cart[index].Product.DONGIA) ?? 0;
                }
                else
                {
                    var product = db.SANPHAM.Find(id);
                    double amount = (quantity * product.DONGIA) ?? 0;
                    cart.Add(new CARTMetadata { Product = product, Quantity = quantity, Amount = amount });
                    Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                }
                Session["cart"] = cart;
                return Json(new { success = true, totalCount = cart.Count });
            }
           }
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            List<CARTMetadata> cart = (List<CARTMetadata>)Session["cart"];
            int index = isExist(id);
            if (index != -1)
            {
                cart[index].Quantity = quantity;

                double amount = quantity * cart[index].Product.DONGIA ?? 0;
                cart[index].Amount = amount;

                Session["cart"] = cart;

                double totalAmount = cart.Sum(item => item.Amount);

                return Json(new { success = true, amount = amount, totalCount = cart.Count }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Remove(int Id)
        {
            List<CARTMetadata> cart = (List<CARTMetadata>)Session["cart"];
            int index = isExist(Id);
            if (index != -1)
            {
                double amountToRemove = cart[index].Amount;
                cart.RemoveAt(index);
                Session["cart"] = cart;
                Session["count"] = Convert.ToInt32(Session["count"]) - 1;
                return Json(new { Message = "Thành công", AmountToRemove = amountToRemove, JsonRequestBehavior.AllowGet });
            }
            else
            {
                return Json(new { Message = "Không thành công", JsonRequestBehavior.AllowGet });
            }
        }

        private int isExist(int id)
        {
            List<CARTMetadata> cart = (List<CARTMetadata>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.IDSP.Equals(id))
                    return i;
            return -1;
        }
    }
}
