using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace electronics_shop.Models
{
    public class ProductDetail
    {
        public Product Product { get; set; }
        public List<ProductImg> productImgsList { get; set; }
    }
}