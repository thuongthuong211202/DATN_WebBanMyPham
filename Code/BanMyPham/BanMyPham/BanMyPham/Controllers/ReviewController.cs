using BanDongHo.Models;
using BanDongHo.Models.EF;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanDongHo.Controllers
{
    public class ReviewController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Review
        [Authorize] // Yêu cầu đăng nhập
        public ActionResult Index()
        {
            return View();
        }
        // review san pham
        // cho phép dụng để cho phép truy cập vào một phương thức hoặc một trang (action hoặc controller) mà không cần xác thực người dùng.
       //  người dùng có thể truy cập vào đó mà không cần đăng nhập hoặc xác thực.
        [AllowAnonymous] 
        public ActionResult _Review( int productId)
        {
            ViewBag.ProductId = productId;
            var item = new ReviewProduct();
            if (User.Identity.IsAuthenticated) // nếu người dùng login
            {
                var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext()); //tạo một đối tượng UserStore mới để quản lý người dùng trong ứng dụng. UserStore sau khi được khởi tạo sẽ cung cấp các phương thức và thuộc tính cho phép  thao tác với người dùng trong cơ sở dữ liệu
                var userManager = new UserManager<ApplicationUser>(userStore); //UserManager là một lớp quan trọng trong việc quản lý người dùng cung cấp một tập hợp các phương thức và thuộc tính để thao tác với người dùng trong cơ sở dữ liệu
                var user = userManager.FindByName(User.Identity.Name); // lay ra ten  người dùng đang đăng nhập
                if(user != null)
                {
                    item.Email = user.Email;
                    item.FullName = user.Fullname;
                    item.UserName = user.UserName;
                }
                return PartialView(item);
            }

            return PartialView();
        }

        // get list đánh giá
        [AllowAnonymous]
        public ActionResult _Load_Review(int productId, int? page)
        {
            var pageSize = 3;
            if (page == null)
            {
                page = 1;
            }
           
            IEnumerable<ReviewProduct> item = _db.Reviews.OrderByDescending(x => x.Id);
             item = _db.Reviews.Where(x => x.ProductId == productId).OrderByDescending(x => x.Id).ToList();
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            item = item.ToPagedList(pageIndex, pageSize);
            return PartialView(item);
        }
        // get list đánh giá
        public ActionResult _LoadAll_Review(int productId, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<ReviewProduct> items = _db.Reviews.Where(x => x.ProductId == productId).OrderByDescending(x => x.Id);
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);

           
        }
        // thêm đánh giá 
        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostReview(ReviewProduct req)
        {
            var dateNow = DateTime.Now;
            if (req.CreateDate == dateNow)
            {
                return Json(new { Success = false });
            }
            if (ModelState.IsValid) //kiểm tra xem dữ liệu được gửi từ giao diện người dùng có hợp lệ hay không, dựa trên các quy tắc xác định trong Model
            {
                req.CreateDate = dateNow;
                _db.Reviews.Add(req);
                _db.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

    }
}