using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BanDongHo.Models.EF
{
    [Table("tb_OrderDetail")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        //Các thuộc tính này cho phép truy cập và gán giá trị của đối tượng Order và đối tượng Product tương ứng
        //virtual cho phép các lớp con ghi đè
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}