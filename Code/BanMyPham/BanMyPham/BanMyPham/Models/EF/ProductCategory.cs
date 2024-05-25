using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BanDongHo.Models.EF
{
    [Table("tb_ProductCategory")]
    public class ProductCategory : CommonAbstract
    {
        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        [Required]
        [StringLength(150)]
        public string Alias { get; set; }
        [StringLength(250)]
        public string Icon { get; set; }
        //Thuộc tính này đại diện cho một tập hợp các đối tượng Product.
        //Có thể sử dụng các phương thức và thuộc tính của ICollection<T> để thao tác với các phần tử trong Products,
        //chẳng hạn như thêm, xóa, hoặc truy vấn các phần tử.
        public ICollection<Product> Products { get; set; }
    }
}