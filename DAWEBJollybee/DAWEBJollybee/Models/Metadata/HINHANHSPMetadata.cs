using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class HINHANHSPMetadata
    {
        [DisplayName("ID ảnh")]
        public int IDHA { get; set; }
        [DisplayName("Mã sản phẩm")]
        public int IDSP { get; set; }
        [DisplayName("Hình ảnh sản phẩm")]       
        public string IMAGE { get; set; }
        public SANPHAM SANPHAM;
    }
}