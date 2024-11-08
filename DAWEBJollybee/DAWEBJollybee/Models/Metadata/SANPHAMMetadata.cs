using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class SANPHAMMetadata
    {
        [DisplayName("Mã sản phẩm")]
        public int IDSP { get; set; }
        [Required(ErrorMessage = ("Bạn chưa chọn mã loại sản phẩm!"))]
        [DisplayName("Mã loại sản phẩm")]
        public Nullable<int> IDLSP { get; set; }
        public LOAISP LOAISP;
        [DisplayName("Hình ảnh")]
        public string HINHANH { get; set; }
        [Required(ErrorMessage = ("Tên sản phẩm không được để trống!"))]
        [DisplayName("Tên sản phẩm")]
        public string TENSP { get; set; }
        [DisplayName("Mô tả")]
        public string MOTA { get; set; }
        [Required(ErrorMessage = ("Đơn giá sản phẩm không được để trống!"))]
        [DisplayName("Đơn giá")]
        [Range(1000, int.MaxValue, ErrorMessage = "Đơn giá phải tối thiểu bằng 1000.")]
        public Nullable<double> DONGIA { get; set; }
        public Nullable<bool> StateProduct { get; set; }
        
    }
}