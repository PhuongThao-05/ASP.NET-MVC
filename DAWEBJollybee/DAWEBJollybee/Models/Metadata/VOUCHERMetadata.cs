using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class VOUCHERMetadata
    {
        [DisplayName("ID khuyến mãi")]
        public int IDCoupon { get; set; }
        [DisplayName("Tên khuyến mãi")]
        [Required(ErrorMessage = ("Bạn chưa điền tên khuyến mãi!"))]
        public string TENVOUCHER { get; set; }
        [DisplayName("Giá trị khuyến mãi")]
        /*[Required(ErrorMessage = ("Bạn chưa điền giá trị khuyến mãi!"))]*/
        [Range(0.05, int.MaxValue, ErrorMessage = "Giá trị phải tối thiểu từ 5% đến 100%.")]
        public Nullable<double> GIATRI { get; set; }
        [DisplayName("Hạn sử dụng")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        //[FutureDate(ErrorMessage = "Ngày hết hạn phải lớn hơn ngày hiện tại.")]
        public Nullable<System.DateTime> HSD { get; set; }
        public Nullable<bool> StateCoupon { get; set; }
        public class FutureDateAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                DateTime? date = value as DateTime?;
                if (date != null)
                {
                    return date > DateTime.Now;
                }
                return true;
            }
        }
    }
}