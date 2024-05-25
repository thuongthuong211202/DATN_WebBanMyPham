using BanDongHo.Models;
using BanDongHo.Models.EF;
using BanDongHo.Models.Payments;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanDongHo.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

  
      
        // lấy tất cả sản phẩm trong giỏ hàng
        public ActionResult Index()
        {

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return View(cart.Items);
                ViewBag.CheckCart = cart;
            }
            return View();
        }


        public ActionResult ShowCount() //hien thi co bao nhieu san pham trong gio hang
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet); //JsonRequestBehavior.AllowGet hỗ trợ cho phương thức GET
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }


       // thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            var db = new ApplicationDbContext();
            var checkProduct = db.Products.FirstOrDefault(x => x.Id == id);
            ShoppingCart cart = (ShoppingCart)Session["Cart"];

            if (cart == null)
            {
                cart = new ShoppingCart();
            }

            ShoppingCartItem item = new ShoppingCartItem
            {
                ProductId = checkProduct.Id,
                ProductName = checkProduct.Title,
                CategoryName = checkProduct.ProductCategory.Title,
                Alias = checkProduct.Alias,
                Quantity = quantity
            };

            if (checkProduct.ProductImages.FirstOrDefault(x => x.IsDefault) != null)
            {
                item.ProductImg = checkProduct.ProductImages.FirstOrDefault(x => x.IsDefault).Image;
            }
            else
            {
                item.ProductImg = null;
            }

            item.Price = checkProduct.Price;

            if (checkProduct.PriceSale > 0)
            {
                item.Price = (decimal)checkProduct.PriceSale; //ép kiểu để phù hợp với kiểu dữ liệu của thuộc tính Price
            }

            item.TotalPrice = item.Quantity * item.Price;

            if (cart.Items.Count < checkProduct.Quantity) //kiểm tra số lượng tồn kho
            {
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart; // lưu trữ thông tin sản phẩm
                code = new { Success = true, msg = "Thêm Sản Phẩm Vào Giỏ Hàng Thành Công!", code = 1, Count = cart.Items.Count };
            }
            else
            {
                code = new { Success = false, msg = "Số lượng trong kho không đủ !!", code = -1, Count = cart.Items.Count };
            }

            return Json(code);
        }

        //update sản phẩm trong giỏ hàng
        [HttpPost]
        public ActionResult Update(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            var checkProduct = db.Products.FirstOrDefault(x => x.Id == id);

            if (cart != null)
            {
                List<ShoppingCartItem> items = cart.Items;
                foreach (var item in items)
                {
                    if (item.ProductId == id)
                    {
                        int itemQuantity = checkProduct.Quantity;
                        if (itemQuantity < quantity)
                        {
                            return Json(new { Success = false, Message = "Số lượng vượt quá số lượng trong kho!!!" });
                            break;
                        }
                        else
                        {
                            cart.UpdateQuantity(id, quantity);
                            return Json(new { Success = true, Message = "Cập nhật thành công" });
                        }
                    }
                }
            }

            return Json(new { Success = false});
        }

        // xóa sản phẩm
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
        }

        // xóa tất cả sản phẩm
        [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
                return Json(new { Success = true });
            }
            return Json(false);
        }

        // khi bấm vào giỏ hàng
        public ActionResult Partial_Item_Cart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        // khi bấm thanh toán
        [Authorize]
        //[HttpPost]
        public ActionResult CheckOut()
        {

            if (User.Identity.IsAuthenticated)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null && cart.Items.Any())
                {
                    ViewBag.CheckCart = cart;
                }
                return View();
            }
            return Json(new { Success = true, msg = "Cần Đăng Nhập Mới Được Mua Hàng" });
        }
        // khi bấm thanh toán thành công
        public ActionResult CheckOutSuccess()
        {

            return View();
        }

        // giao diện xác thực thông tin khách hàng và đơn hàng khi bấm thanh toán
        public ActionResult Partial_CheckOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();//ang thực hiện việc lấy đối tượng UserManager
                var userId = User.Identity.GetUserId();
                var user = _userManager.FindById(userId);
                var userInfo = new OrderViewModel // tạo đối tượng và gán giá trị cho đối tượng 
                {
                    Email = user.Email,
                    CustomerName = user.Fullname,
                    Phone = user.Phone,
                    Address = user.Address,
                    TypePayment = 0,
                    TypePaymentVN = 0
                };
                return PartialView(userInfo);
            }
            else
            {
                var userInfo = new OrderViewModel
                {
                    Phone = "N/A",
                    Email = "N/A",
                    CustomerName = "N/A"
                };
                return PartialView(userInfo);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //khi chọn thanh toán trực tuyến
        public ActionResult CheckOut(OrderViewModel req)
        {

            var code = new { Success = false, Code = -1, Url = "" };
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart.Items.Count == 0)
                {
                    return RedirectToAction("CheckOutSuccess");
                }
                else if (cart != null)
                {
                    Order order = new Order();
                    order.CustomerName = req.CustomerName;
                    order.Phone = req.Phone;
                    order.Address = req.Address;
                    order.Email = req.Email;
                    order.Status = 1;//chưa thanh toán / 2/đã thanh toán, 3/Hoàn thành, 4/hủy
                    cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        Price = x.Price
                    }));
                    // Giảm số lượng sản phẩm trong cơ sở dl
                    foreach (var item in order.OrderDetails)
                    {
                        var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                        if (product != null)
                        {
                            product.Quantity -= item.Quantity;
                        }
                    }
                    order.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));// tổng tiền
                    order.TypePayment = req.TypePayment; // trang thái thanh toán
                    order.CreateDate = DateTime.Now;
                    order.ModifiedDate = DateTime.Now;
                    order.CreateBy = req.Phone;
                    Random rd = new Random();
                    order.Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                    //order.E = req.CustomerName;
                    db.Orders.Add(order);
                    db.SaveChanges();
                    cart.ClearCart(); // xóa tất cả sản phẩm trong giỏ hàng khi đã bấm thanh toán
                    code = new { Success = true, Code = req.TypePayment, Url = "" };
                    //var url = "";
                    if (req.TypePayment == 2)
                    {
                        var url = UrlPayment(req.TypePaymentVN, order.Code);
                        code = new { Success = true, Code = req.TypePayment, Url = url };
                    }


                }
            }
            return Json(code);
        }

        // sau khi thanh toán thẻ nội địa thành công
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));//cho phép bạn sử dụng giá trị của vnpayTranId trong việc xử lý và lưu trữ thông tin về mã giao dịch từ phản hồi của VNPAY.
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");//cho phép bạn sử dụng giá trị của vnp_ResponseCode trong việc xử lý và kiểm tra mã phản hồi từ VNPAY để xác định kết quả của giao dịch thanh toán.
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                //cho phép bạn sử dụng giá trị của vnp_TransactionStatus trong việc xử lý và kiểm tra trạng thái giao dịch thanh toán từ phản hồi của VNPAY.
                //Ví dụ: giá trị "00" có thể đại diện cho giao dịch thành công
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; 
                String TerminalID = Request.QueryString["vnp_TmnCode"];//truy cập các tham số truy vấn (query string) trong URL của yêu cầu,sử dụng giá trị của TerminalID trong việc xử lý và lưu trữ thông tin liên quan đến mã Terminal
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100; // tổng tiền
                String bankCode = Request.QueryString["vnp_BankCode"];// tên ngân hàng

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);//kiểm tra tính hợp lệ của chữ ký
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.Orders.FirstOrDefault(x => x.Code == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.Status = 2;//đã thanh toán
                            db.Orders.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Giao dịch không thành công vui lòng thử lại.Mã lỗi " + vnp_ResponseCode;
                        
                    }
                
                }
            }
            return View();

        }

        #region Thanh toán vnpay
        /*TypePaymentVN là kiểu thanh toán ,orderCode là mã của đơn hàng */
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.Orders.FirstOrDefault(x => x.Code == orderCode);//tìm kiếm trong phần hóa đơn
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalAmount * 100; // để số không có dấu phẩy
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK"); // Mã Ngân hàng thanh toán. Ví dụ: NCB
            }
            vnpay.AddRequestData("vnp_CreateDate", order.CreateDate.ToString("yyyyMMddHHmmss")); /*Ngày tạo hóa đơn*/
            vnpay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());//Địa chỉ IP của khách hàng thực hiện giao dịch
            vnpay.AddRequestData("vnp_Locale", "vn");//Ngôn ngữ giao diện hiển thị. Hiện tại hỗ trợ Tiếng Việt (vn)
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);//Thông tin mô tả nội dung thanh toán (Tiếng Việt, không dấu).
            vnpay.AddRequestData("vnp_OrderType", "other"); //Mã danh mục hàng hóa. Mỗi hàng hóa sẽ thuộc một nhóm danh mục do VNPAY quy định. Xem thêm bảng Danh mục hàng hóa

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);//URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán.
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);//để tạo URL yêu cầu thanh toán
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion
    }
}