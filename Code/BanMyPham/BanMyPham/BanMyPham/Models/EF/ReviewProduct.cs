﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BanDongHo.Models.EF
{
    [Table("tb_Review")]
    public class ReviewProduct
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public int Rate { get; set; }/*sao*/
        public DateTime CreateDate { get; set; }
        public string Avatar { get; set; }
        public virtual Product Product { get; set; }/*1 sanr phamar có nhiều lượt review*/
    }
}