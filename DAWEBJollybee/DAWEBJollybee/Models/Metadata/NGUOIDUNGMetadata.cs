using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class NGUOIDUNGMetadata
    {
        [Required(ErrorMessage = ("Tên tài khoản không được để trống!"))]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Username phải dài tối thiểu nhất 6 kí tự!")]
        public string USERNAME { get; set; }
        [Required(ErrorMessage = ("Mật khẩu không được để trống!"))]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải dài tối thiểu nhất 6 kí tự!")]
        public string PASSWORD { get; set; }
        public Nullable<bool> IDENTIFY { get; set; }
    }
}