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
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Statistical
        public ActionResult Index()
        {

            return View();
        }

        // lấy dữ liệu hóa đơn 
        [HttpGet]
        public ActionResult GetStatistical(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate == null || toDate == null)
            {
                var queryy = from o in db.Orders
                             join od in db.OrderDetails
                             on o.Id equals od.OrderId
                             join p in db.Products
                             on od.ProductId equals p.Id
                             select new
                             {
                                 CreatedDate = o.CreateDate, // ngày đặt đơn hàng
                                 Quantity = od.Quantity, // số lượng
                                 Price = od.Price, // giá
                                 OriginalPrice = p.OriginalPrice // giá nhập
                             };
                //TruncateTime bỏ thời gian đi lấy ngày thôi
                var resultt = queryy.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
                {
                    Date = x.Key.Value,
                    TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice), // tổng tiền nhập máy
                    TotalSell = x.Sum(y => y.Quantity * y.Price), // tổng tiền bán
                }).OrderByDescending(x => x.Date).Select(x => new
                {
                    Date = x.Date,
                    DoanhThu = x.TotalSell,
                    LoiNhuan = x.TotalSell - x.TotalBuy
                });
                return Json(new { Data = resultt }, JsonRequestBehavior.AllowGet);
            }else {
                var query = from o in db.Orders
                            join od in db.OrderDetails
                             on o.Id equals od.OrderId
                            join p in db.Products
                            on od.ProductId equals p.Id
                            where o.CreateDate > fromDate && o.CreateDate <= toDate
                            select new
                            {
                                CreatedDate = o.CreateDate, // ngày đặt đơn hàng
                                Quantity = od.Quantity, // số lượng
                                Price = od.Price, // giá
                                OriginalPrice = p.OriginalPrice // giá nhập
                            };
                //TruncateTime bỏ thời gian đi lấy ngày thôi
                var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
                {
                    Date = x.Key.Value,
                    TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice), // tổng tiền nhập máy
                    TotalSell = x.Sum(y => y.Quantity * y.Price), // tổng tiền bán
                }).OrderByDescending(x => x.Date).Select(x => new
                {
                    Date = x.Date,
                    DoanhThu = x.TotalSell,
                    LoiNhuan = x.TotalSell - x.TotalBuy
                });
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);/*cho phép yêu cầu GET để truy xuất dữ liệu JSON.*/
            }
           
        }
    }
}