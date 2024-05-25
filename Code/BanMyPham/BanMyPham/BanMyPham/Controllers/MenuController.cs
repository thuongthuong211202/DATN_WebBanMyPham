using BanDongHo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanDongHo.Controllers
{
    public class MenuController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        /*hiển thị danh mục trong header trang chủ */
        public ActionResult MenuTop()
        {
            var items = db.Categories.OrderBy(x => x.Position).ToList();
            return PartialView("_MenuTop", items);
        }

    
       /*phần Thương hiệu trong trang sản phẩm*/
        public ActionResult MenuLeft(int? id)
        {
            var items = db.ProductCategories.ToList();
            if (id != null)
            {
                ViewBag.CateId = id;
            }
            return PartialView("_MenuLeft", items);
        }

     
    }
}