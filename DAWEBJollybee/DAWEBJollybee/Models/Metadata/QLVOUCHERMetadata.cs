using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class QLVOUCHERMetadata
    {
        [DisplayName("ID khuyến mãi")]
        public int IDCoupon { get; set; }
        [DisplayName("ID khách hàng")]
        public int IDKH { get; set; }
        public Nullable<bool> StateUse { get; set; }
        public Nullable<System.DateTime> NGAYTHEM { get; set; }
        public Nullable<System.DateTime> NGAYSD { get; set; }
        public VOUCHER VOUCHER;
    }
}