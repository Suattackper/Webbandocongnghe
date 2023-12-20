using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace electronics_shop.Models
{
    public class ProductDetail
    {
        public Product Product { get; set; }
        public Promotion Promotion { get; set; }
        public List<ProductImg> ProductImgs { get; set; }
        public Category Category { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Account> Accounts { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public List<Promotion> RelatedPromotions { get; set; }
        public double Rate1 { get; set; }
        public double Rate2 { get; set; }
        public double Rate3 { get; set; }
        public double Rate4 { get; set; }
        public double Rate5 { get; set; }
        //public List<ProductImg> productImgsList { get; set; }
    }
}