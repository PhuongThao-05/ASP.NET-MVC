using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class LOAISPMetadata
    {
        [DisplayName("Mã loại sản phẩm")]
        public int IDLSP { get; set; }
        [Required(ErrorMessage = ("Tên loại hàng không được để trống!"))]
        [DisplayName("Tên loại sản phẩm")]
        public string TENLOAISP { get; set; }
    }
}