using BanDongHo.Models;
using BanDongHo.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanDongHo.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // danh sách sản phẩm và  phân trang
        public ActionResult Index(string Searchtext, int? id, int? page)
        {
            var pageSize = 8;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.Alias.Contains(Searchtext) || x.Title.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        //danh sách thương hiệu sản phẩm của trang "SẢN PHẨM"
        public ActionResult ProductCategory(string alias, int id)
        {
            var items = db.Products.ToList();
            if (id > 0)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }
            ViewBag.CateId = id;
            return View(items);
        }

        // sản phẩm bán chạy ở trang chủ
        public ActionResult Partial_ProductSale()
        {
            var items = db.Products.Where(x => x.IsSale && x.IsActive).Take(16).ToList();
            return PartialView(items);
        }

        //chi tiết sản phẩm
        public ActionResult Detail(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                if (item.ViewCount == 1000)
                {
                    item.ViewCount = 0;
                }
                db.Entry(item).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
            return View(item);
        }

      
      }
}