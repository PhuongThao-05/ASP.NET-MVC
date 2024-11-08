using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class KHACHHANGMetadata
    {
        [DisplayName("ID")]
        public int IDKH { get; set; }
        [DisplayName("Tên tài khoản")]
        [Required(ErrorMessage = ("Tên tài khoản không được để trống!"))]
        public string USERNAME { get; set; }
        [DisplayName("Họ và tên")]
        [Required(ErrorMessage = ("Họ tên không được để trống!"))]
        public string HOTEN { get; set; }
        [DisplayName("Email")]
        [Required(ErrorMessage = ("Email không được để trống!"))]
        public string EMAIL { get; set; }
        [DisplayName("Số điện thoại")]
        [Required(ErrorMessage = ("Số điện thoại không được để trống!"))]
        public string SDT { get; set; }
        [DisplayName("Địa chỉ")]
        [Required(ErrorMessage = ("Địa chỉ không được để trống!"))]
        public string DIACHI { get; set; }
    }
}