using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace electronics_shop.Models
{
    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; }
        public ShoppingCart()
        {
            this.Items = new List<ShoppingCartItem>();

        }
        public void AddToCart(ShoppingCartItem item, int Quantity)
        {
            var checkExits = Items.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (checkExits != null)
            {
                // tang so luong , thay doi gia
                checkExits.Quantity += Quantity;
                checkExits.ToTalPrice = checkExits.Price * checkExits.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void Remove( string id)
        {
            var checkExits = Items.SingleOrDefault(x =>x.ProductId == id);
            if (checkExits != null)
            {
                Items.Remove(checkExits);
             
            }
        }

        public void UpdateQuantity(string id, int quantity)
        {
            var checkExits = Items.SingleOrDefault(x => x.ProductId == id);
            if(checkExits!=null)
            {
                checkExits.Quantity = quantity;
                checkExits.ToTalPrice = checkExits.Price * checkExits.Quantity;
            }
        }
        public double GetTotalPrice()
        {
            return Items.Sum(x => x.ToTalPrice);
        }
        public int GetTotalQuan()
        {
            return Items.Sum(x => x.Quantity);
        }

        public void ClearCart()
        {
            Items.Clear();
        }
    }

    public class ShoppingCartItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string ProductImg { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double PromotionPrice { get; set; }
         public double ToTalPrice { get; set; }
    }
}