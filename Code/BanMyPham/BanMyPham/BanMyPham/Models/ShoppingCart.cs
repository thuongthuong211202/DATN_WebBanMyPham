using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanDongHo.Models
{
    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; }
        public ShoppingCart()
        {
            this.Items = new List<ShoppingCartItem>();
        }

        // thêm sản phẩm vào giỏ hàng (ktra xem trong giỏ hàng nếu đã có sản phẩm thì update sl và tiền)
        public void AddToCart(ShoppingCartItem item, int Quantity)
        {
            var checkExits = Items.FirstOrDefault(x => x.ProductId == item.ProductId); // tìm kiếm ptu đàu tiên trong dsach
            if (checkExits != null)
            {
                checkExits.Quantity += Quantity;
                checkExits.TotalPrice = checkExits.Price * checkExits.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        // xóa sản phẩm khỏi giỏ hàng
        public void Remove(int id)
        {
            var checkExits = Items.SingleOrDefault(x => x.ProductId == id);
            if (checkExits != null)
            {
                Items.Remove(checkExits);
            }
        }

        // cập nhật sản phẩm 
        public void UpdateQuantity(int id, int quantity)
        {
            var checkExits = Items.SingleOrDefault(x => x.ProductId == id);
            if (checkExits != null)
            {
                checkExits.Quantity = quantity;
                checkExits.TotalPrice = checkExits.Price * checkExits.Quantity;
            }
        }


        // tính tổng tiền sản phẩm trong giỏ hàng
        /*public decimal GetTotalPrice()
        {
            return Items.Sum(x => x.TotalPrice);
        }

        public decimal GetTotalQuantity()
        {
            return Items.Sum(x => x.Quantity);
        }*/

        // xóa tất cả sản phẩm có trong giỏ hàng
        public void ClearCart()
        {
            Items.Clear();
        }
    }

    public class ShoppingCartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Alias { get; set; }
        public string CategoryName { get; set; }
        public string ProductImg { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
}