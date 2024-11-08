using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class DONHANGMetadata
    {
        [DisplayName("Mã đơn hàng")]
        public int IDDH { get; set; }
        [DisplayName("IDKH")]
        public int IDKH { get; set; }
        public KHACHHANG KHACHHANG;
        [DisplayName("Ngày đặt")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> NGAYDAT { get; set; }
        [DisplayName("PTTT")]
        public Nullable<int> IDPAYMENT { get; set; }
        public PAYMENTMETHOD PAYMENTMETHOD;
        [DisplayName("Chi phí dự kiến")]    
        public Nullable<double> TONGTIEN { get; set; }
        [DisplayName("Phí ship")]     
        public Nullable<double> PHISHIP { get; set; }
        [DisplayName("Khuyến mãi")]     
        public Nullable<double> KHUYENMAI { get; set; }
        [DisplayName("Tổng tiền thanh toán")]    
        public Nullable<double> THANHTIEN { get; set; }
        [DisplayName("Trạng thái đơn hàng")]
        public string LastestState { get; set; }
        [DisplayName("Hoàn thành")]
        public Nullable<bool> HOANTAT { get; set; }
        [DisplayName("Hủy")]
        public Nullable<bool> HUY { get; set; }
        public Nullable<bool> DeleteOrder { get; set; }
        public Nullable<int> IDCoupon { get; set; }
        public TRANGTHAIDH TRANGTHAIDH;
    }
}