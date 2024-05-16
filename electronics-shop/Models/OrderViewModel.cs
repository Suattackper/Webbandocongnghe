using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace electronics_shop.Models
{
    public class OrderViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string Number { get; set; }
        public int TypePayment { get; set; }
        public int TypePaymentVN { get; set; }
    }
}