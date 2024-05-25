using BanDongHo.Models;
using BanDongHo.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanDongHo.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // hiển thị danh sách hóa đơn
        public ActionResult Index(DateTime? searchDate, int? page)
        {
            IEnumerable<Order> items = db.Orders.OrderByDescending(x => x.Id);

           
            if (page == null)
            {
                page = 1;
            }       
            if(searchDate != null)
            {
                items = db.Orders.OrderByDescending(x => x.CreateDate).ToList();
                var startDate = searchDate.Value.Date;
             items = db.Orders.Where(e => e.CreateDate.Day == startDate.Day).ToList();
            }
           
            var pageSize = 5;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        // hiển thị chi tiết đơn hàng
        public ActionResult ViewOrder(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        // danh sách sản phẩm hiển thị trang chi tiết hóa đơn
        public ActionResult Partial_SanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(items);
        }

        // update trạng thái đơn hàng
        [HttpPost]
        public ActionResult UpdateTT(int id, int trangthai)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                db.Orders.Attach(item);
                item.Status = trangthai;
                db.Entry(item).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "UnSuccess", Success = false });
        }
    }
}