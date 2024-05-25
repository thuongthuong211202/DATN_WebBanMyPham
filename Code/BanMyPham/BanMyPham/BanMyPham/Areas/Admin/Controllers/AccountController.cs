using BanDongHo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BanDongHo.Areas.Admin.Controllers
{
    /*[Authorize(Roles = "Admin")]*/
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private IEnumerable<object> roles;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Admin/Account
        public async Task<ActionResult> Index(int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IEnumerable<UserViewModel> item = (from u in db.Users
                                               select new UserViewModel
                                               {
                                                   Id = u.Id,
                                                   UserName = u.UserName,
                                                   FullName = u.Fullname,
                                                   Phone = u.Phone,
                                                   Email = u.Email,
                                                   Role = (from userRole in u.Roles
                                                           join role in db.Roles on userRole.RoleId equals role.Id
                                                           select role.Name).ToList()
                                               }).ToList();
            /*IEnumerable<UserViewModel> item = (IEnumerable<UserViewModel>)db.Users.ToList();*/
            item = item.OrderByDescending(x => x.Id);
            item = item.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(item);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            
            return View();
        }

        //
        //Thêm mới tài khoản
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Fullname = model.FullName,
                    Phone = model.Phone,/*
                    Role = model.Role*/
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, model.Role); // Lưu ý dòng này khi đăng ký thành công để nó add vào table UserRole
                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        /*sửa tài khoản admin*/
        public ActionResult Edit(string id)
        {
         
            var item = db.Users.Find(id);
            var selecList = new SelectList(db.Roles, "Name", "Name", item.Id);
            ViewBag.Role = selecList;
            return View(item);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var oldItem = db.Users.Find(model.Id);
                oldItem.UserName = model.UserName;
                oldItem.Fullname = model.Fullname;
                oldItem.Phone = model.Phone;
                oldItem.Email = model.Email;
                oldItem.RoleNames = model.RoleNames;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);   
        }

        // xoa tài khoản admin
        [HttpPost]
        public async Task<ActionResult> DeleteAccount(string user, string id)
        {
            
            var code = new { success = false };//mac định là xóa thành thông
            //ktra xem user co ton tai khong
            var item = UserManager.FindByName(user);
            if(item != null)
            {
                var rolesForUser = await UserManager.GetRolesAsync(id);
                //var rolesForUser = await UserManager.GetRolesAsync(id);
                if(rolesForUser != null)
                {
                    foreach(var role in rolesForUser)
                    {
                        await UserManager.RemoveFromRoleAsync(id, role);
                    }
                }
               var res =await UserManager.DeleteAsync(item);
                code = new { success = res.Succeeded };
            }
            return Json(code);
        }
        /*----------------------- đăng nhập --------------------------------*/

        [Authorize(Roles = "Admin,Employee")]
        // đăng nhập admin
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [Authorize(Roles = "Admin,Employee")]
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Để kích hoạt lỗi mật khẩu nhằm kích hoạt khóa tài khoản, hãy đổi thành ShouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                    return View(model);
            }
        }
        // login thành công đăng nhập vào giao dện admin
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }


}