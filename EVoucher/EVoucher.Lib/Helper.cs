using EVoucher.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using static Swastika.Common.Utility.Enums;

namespace EVoucher.Lib
{
    public class BSHelper
    {
        public static string GenerateCode()
        {
            string allowed = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = string.Empty;
            Random rnd = new Random();
            for (int i = 0; i < 6; i++)
            {
                result += allowed[rnd.Next(0, allowed.Length - 1)];
            }
            return result;
        }
        public static async Task<string> SendMessage(int status, string phone, string code, string bid = null)
        {
            string message = GetMessage(status, phone, code);

            string account = "bst_turanza";// "Bridgestonevn";
            string passcode = "6p3ma";// "tvk2m";
            string serviceId = status == 0 ? "Bridgestone" : "6067";
            string url = $"http://cloudsms.vietguys.biz:8088/api/?u={account}&pwd={passcode}&from={serviceId}&phone={phone}&sms={message}&bid={bid}";
            //string url = $"http://sms.vietguys.biz/api/?u={account}&pwd={passcode}&from={serviceId}&phone={phone}&sms={message}&bid={bid}";
            using (var client = new HttpClient())
            {
                var tokenResponse = client.GetAsync(url).Result;
                return await tokenResponse.Content.ReadAsStringAsync();
            }
        }

        public static string GetMessage(int status, string phone, string code)
        {
            DateTime startDate = new DateTime(2018, 06, 19);
            DateTime endDate = new DateTime(2018, 07, 20);
            string message = string.Empty;
            switch (status)
            {
                case 0:
                    message = $"Bridgestone Viet Nam gui ban ma: {code}. " +
                        $"Ma co gia tri quy doi 01 the xang Flexicard tri gia 300K khi mua 02 lop Turanza GR100 " +
                        $"tai he thong dai ly cua Bridgestone tren toan quoc. Hieu luc den { endDate.ToString("dd/MM/yyyy") }. " +
                        $"Hotline: 1900545468";
                    break;
                case -1:
                    message = $"Thoi gian tham du chuong trinh Turanza se bat dau tu ngay " +
                        $"{ startDate.ToString("dd/MM/yyyy") } den ngay { endDate.ToString("dd/MM/yyyy") }. " +
                        $"Moi thac mac xin lien he 1900545468. Xin cam on.";
                    break;
                case -2:
                    var register = BSRegisterViewModel.Repository.GetSingleModel(m => m.Phone == phone && m.Status != (int)SWStatus.Deleted).Data;
                    message = $"SDT cua Quy khach da duoc su dung tham gia chuong " +
                        $"trinh ngay { register?.CreatedDate.ToString("dd/MM") } va khong the tiep tuc tham gia. " +
                        $"Cam on Quy khach da dong hanh cung Bridgestone. LH:1900545468";
                    break;
                case -3:
                    message = $"Chuong trinh tang the xang 300K khi mua 02 lop Turanza GR100 " +
                        $"da ket thuc tu ngay { endDate.ToString("dd/MM/yyyy") }. " +
                        $"Cam on Quy khach da dong hanh cung Bridgestone. Hotline 1900545468";
                    break;
                default:
                    message = $"Nhà mạng sẽ tự động gửi nội dung thông báo cú pháp sai (KH không thể điều chỉnh nội dung này). " +
                        $"Đồng thời, M&C cũng không có báo cáo / thông tin về các tin này";
                    break;
            }
            return HttpUtility.UrlEncode(message);
        }
        public static string ParsePhone(string phone)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(phone))
            {

                if (phone[0] == '0')
                {
                    result = phone.Substring(1);
                }
                else
                {
                    if (phone[0] == '8')
                    {
                        result = phone.Substring(2);
                    }
                    else if (phone[0] == '+')
                    {
                        result = phone.Substring(3);
                    }
                }
            }
            return result;
        }
        public static int ValidateRegister(string phone)
        {
            
            DateTime startDate = new DateTime(2018, 06, 19);
            DateTime endDate = new DateTime(2018, 07, 20);

            if (DateTime.Now < startDate)
            {
                return -1;
            }
            if (DateTime.Now > endDate)
            {
                return -3;
            }
            if (!BSHelper.IsPhoneNumber(phone))
            {
                return -4;
            }
            if (BSRegisterViewModel.Repository.CheckIsExists(m => m.Phone.Contains(ParsePhone(phone)) && m.Status != (int)SWStatus.Deleted))
            {
                return -2;
            }
            return 0;
        }

        public static bool IsPhoneNumber(string number)
        {
            return long.TryParse(number, out long t);
        }
        public async Task<string> PostMessage(string phone, string message)
        {
            //Không SSL: http://cloudsms.vietguys.biz:8088/webservices/sendsmsw.php?wsdl
            //Có SSL: https://cloudsms.vietguys.biz:4438/webservices/sendsmsw.php?wsdl

            string url = "http://cloudsms.vietguys.biz:8088/webservices/sendsmsw.php?wsdl";
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"account", "bst_turanza"},//"Bridgestonevn"
                {"passcode", "6p3ma"},//"tvk2m"
                {"service_id", "Bridgestone"},
                {"phone", phone},
                {"sms", message}
                //{"transactionid", "" }
            };

            return await PostDataAsync(url, data);
        }
        public async Task<string> PostDataAsync(string url, Dictionary<string, string> data)
        {
            using (var client = new HttpClient())
            {
                var tokenResponse = client.PostAsync(url, new FormUrlEncodedContent(data)).Result;
                return await tokenResponse.Content.ReadAsStringAsync();
            }
        }

    }
}
