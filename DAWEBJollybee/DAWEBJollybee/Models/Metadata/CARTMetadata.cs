using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace DAWEBJollybee.Models.Metadata
{
    public class CARTMetadata
    {
        public double total { get; set; }
        public SANPHAM Product { get; set; }

        public int Quantity { get; set; }

        public double Amount { get; set; }


    }
}