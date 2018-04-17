using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
        public static async Task<string> SendMessage(string phone, string code)
        {
            string message = $"Bridgestone Việt Nam \r\n";
            message += "Quy khach vua dang ki chuong trinh Khuyen mai cua Bridgestone.\r\n";
            message += $"Mã ưu đãi: {code}\r\n";
            message += "Co gia tri giam 300 nghin dong tren phi dich vu khi mua 02 lop Turanza GR100 tai he thong B-select, B-shop cua Bridgestone tren ca nuoc.\r\n";
            message += "Ma uu dai co Hieu luc tu 15/04 den 30/04/2018\r\n";
            message += "De biet them chi tiet quy khach vui long goi den hotline: 1900 54 54 68 de biet them chi tiet.\r\n";
            message += "Xin cam on.";

            string account = "Bridgestonevn";
            string passcode = "tvk2m";
            string serviceId = "Bridgestone";
            string url = $"http://sms.vietguys.biz/api/?u={account}&pwd={passcode}&from={serviceId}&phone={phone}&sms={message}";
            using (var client = new HttpClient())
            {
                var tokenResponse = client.GetAsync(url).Result;
                return await tokenResponse.Content.ReadAsStringAsync();
            }
        }
        public async Task<string> PostMessage(string phone, string message)
        {
            //Không SSL: http://cloudsms.vietguys.biz:8088/webservices/sendsmsw.php?wsdl
            //Có SSL: https://cloudsms.vietguys.biz:4438/webservices/sendsmsw.php?wsdl

            string url = "http://cloudsms.vietguys.biz:8088/webservices/sendsmsw.php?wsdl";
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"account", "Bridgestonevn"},
                {"passcode", "tvk2m"},
                {"service_id", "Bridgestone"},
                {"phone", phone},
                {"sms", message},
                {"transactionid", "" }
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
