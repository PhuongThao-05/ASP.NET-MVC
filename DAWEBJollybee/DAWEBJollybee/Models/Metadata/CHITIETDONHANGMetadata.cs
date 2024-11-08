using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class CHITIETDONHANGMetadata
    {
        [DisplayName("Mã đơn hàng")]
        public int IDDH { get; set; }
        [DisplayName("Mã sản phẩm")]
        [Required(ErrorMessage = ("Bạn chưa chọn sản phẩm!"))]
        public int IDSP { get; set; }
        public SANPHAM SANPHAM;
        [DisplayName("Số lượng")]
        [Required(ErrorMessage = ("Bạn chưa nhập số lượng!"))]
        public Nullable<int> SOLUONG { get; set; }
        [DisplayName("Giá tiền")]
        public Nullable<double> GIATIEN { get; set; }
    }
}