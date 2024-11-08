using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class TRANGTHAIDHMetadata
    {
        [DisplayName("ID trạng thái")]
        public int IDSTATE { get; set; }
        [DisplayName("Mã đơn hàng")]
        public int IDDH { get; set; }
        [DisplayName("Trạng thái hiện tại")]
        public string CurrentState { get; set; }
        [DisplayName("Thời gian")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StateTime { get; set; }
        public DONHANG DONHANG;
    }
}