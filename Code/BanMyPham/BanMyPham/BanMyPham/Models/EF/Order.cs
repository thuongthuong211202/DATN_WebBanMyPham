using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BanDongHo.Models.EF
{
    [Table("tb_Order")]
    public class Order : CommonAbstract
    {
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();//tạo một đối tượng mới của lớp HashSet<OrderDetail> và gán nó cho thuộc tính OrderDetails của đối tượng hiện tại.
            //OrderDetails có thể là một thuộc tính hoặc một trường của lớp hiện tại.
            //đối tượng hiện tại sẽ có một tập hợp rỗng để lưu trữ các đối tượng OrderDetail
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required(ErrorMessage = "Tên Khách Hàng Không Được Để Trống")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Số Điện Thoại Không Được Để Trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa Chỉ Không Được Để Trống")]
        public string Address { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public int TypePayment { get; set; }
        public int Status { get; set; }
        //khai báo một thuộc tính có tên OrderDetails trong lớp hiện tại. có kiểu dữ liệu  OrderDetail
        //Thuộc tính này có thể được truy cập từ bất kỳ đâu và có thể được ghi đè trong các lớp con.
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}