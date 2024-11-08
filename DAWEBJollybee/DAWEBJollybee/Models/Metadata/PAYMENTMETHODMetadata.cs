using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAWEBJollybee.Models.Metadata
{
    public class PAYMENTMETHODMetadata
    {
        [DisplayName("ID phương thức")]
        public int IDPAYMENT { get; set; }
        [DisplayName("Phương thức thanh toán")]
        [Required(ErrorMessage = ("Bạn chưa điền tên phương thức thanh toán!"))]
        public string METHODNAME { get; set; }
    }
}