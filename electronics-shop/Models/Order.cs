//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace electronics_shop.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }
    
        public int OrderCode { get; set; }
        public Nullable<int> PaymentCode { get; set; }
        public Nullable<int> AccountCode { get; set; }
        public string PromotionCode { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<bool> Delivered { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<decimal> OrderTotal { get; set; }
        public string OrderNote { get; set; }
        public Nullable<int> AccountAddressCode { get; set; }
        public string DeliveryCode { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual AccountAddress AccountAddress { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}
