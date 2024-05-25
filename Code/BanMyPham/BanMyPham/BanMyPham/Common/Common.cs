using BanDongHo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace BanDongHo.Common
{
    public class Common
    {
        /*gửi mail reset password*/
        private static string Fr_Email = ConfigurationManager.AppSettings["Email"];
        private static string password = ConfigurationManager.AppSettings["Password"];
        public static bool SendMail(string name, string content,string To_Email)
        {
            bool rs = false;
            try
            {
                MailMessage message = new MailMessage();
                var smtp = new SmtpClient(); //Tạo một đối tượng SmtpClient để gửi email.
                {
                    smtp.Host = "smtp.gmail.com"; //Đặt tên máy chủ SMTP
                    smtp.Port = 587; //Đây là cổng tiêu chuẩn được sử dụng cho giao thức SMTP.
                    smtp.EnableSsl = true; //Kích hoạt chế độ SSL cho máy chủ SMTP. Điều này yêu cầu kết nối được mã hóa SSL khi gửi email.
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network; //Xác định phương thức gửi email là thông qua mạng.

                    smtp.UseDefaultCredentials = false; //Không sử dụng thông tin đăng nhập mặc định.
                     smtp.Credentials = new NetworkCredential() //Đặt thông tin đăng nhập
                     {
                        UserName = Fr_Email,
                        Password = password
                    };
                }
                MailAddress fromAddress = new MailAddress(Fr_Email, name); //Tạo một đối tượng MailAddress để chỉ định địa chỉ email nguồn và tên người gửi.
                message.From = fromAddress; //Đặt địa chỉ email nguồn của thư.
                message.To.Add(To_Email);//Thêm địa chỉ email đích vào danh sách người nhận.
                message.IsBodyHtml = true;//cho phép nội dung mail được hiển thị dưới dạng HTML.
                message.Body = content; // nội dung mail
                smtp.Send(message);
                rs = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                rs = false;
            }
            return rs;
        }
        public static string FormatNumber(object value, int SoSauDauPhay = 2)
        {
            bool isNumber = IsNumeric(value);
            decimal GT = 0;
            if (isNumber)
            {
                GT = Convert.ToDecimal(value);
            }
            string str = "";
            string thapPhan = "";
            for (int i = 0; i < SoSauDauPhay; i++)
            {
                thapPhan += "#";
            }
            if (thapPhan.Length > 0) thapPhan = "." + thapPhan;
            string snumformat = string.Format("0:#,##0{0}", thapPhan);
            str = String.Format("{" + snumformat + "}", GT);

            return str;
        }
        private static bool IsNumeric(object value)
        {
            return value is sbyte
                       || value is byte
                       || value is short
                       || value is ushort
                       || value is int
                       || value is uint
                       || value is long
                       || value is ulong
                       || value is float
                       || value is double
                       || value is decimal;
        }


        public static string HtmlRate(int rate)
        {
            var str = "";
            if(rate == 1)
            {
                str = @"
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 2)
            {
                str = @"
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 3)
            {
                str = @"
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 4)
            {
                str = @"
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 5)
            {
                str = @"
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star' aria-hidden='true'></i></li>
                <li><i class='fa fa-star' aria-hidden='true'></i></li>";
            }
            return str;
        }
    }
}