using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BanDongHo.Models.EF
{
    [Table("tb_Category")]
    public class Category : CommonAbstract
    {
       
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên danh mục không để trống")]
        [StringLength(150)]
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Link { get; set; }

        public bool IsActive { get; set; }
        public int Position { get; set; }
       
    }
}