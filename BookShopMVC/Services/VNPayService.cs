using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BookShopMVC.Services
{
    public class VNPayService
    {
        private readonly IConfiguration _config;

        public VNPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext httpContext, decimal amount, string orderId, string orderInfo)
        {
            var timeNow = DateTime.Now;
            var vnp_TmnCode = _config["VNPay:vnp_TmnCode"] ?? throw new Exception("Thiếu cấu hình vnp_TmnCode");
            var vnp_HashSecret = _config["VNPay:vnp_HashSecret"] ?? throw new Exception("Thiếu cấu hình vnp_HashSecret");
            var vnp_Url = _config["VNPay:vnp_Url"] ?? throw new Exception("Thiếu cấu hình vnp_Url");
            var vnp_ReturnUrl = _config["VNPay:vnp_ReturnUrl"] ?? throw new Exception("Thiếu cấu hình vnp_ReturnUrl");

            var pay = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", ((long)(amount * 100)).ToString() },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", orderId },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "billpayment" },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", vnp_ReturnUrl },
                { "vnp_IpAddr", GetIpAddress(httpContext) },
                { "vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss") }
            };

            
            string rawData = string.Join("&", pay.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));
            string secureHash = CreateSecureHash(vnp_HashSecret, pay);
            string queryString = string.Join("&", pay.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));
            string paymentUrl = $"{vnp_Url}?{queryString}&vnp_SecureHash={secureHash}";
           

            return paymentUrl;
        }

        private string CreateSecureHash(string secretKey, SortedDictionary<string, string> pay)
        {
            if (pay.ContainsKey("vnp_SecureHash")) pay.Remove("vnp_SecureHash");
            if (pay.ContainsKey("vnp_SecureHashType")) pay.Remove("vnp_SecureHashType");

            var sorted = pay.OrderBy(x => x.Key);
            string rawData = string.Join("&", sorted.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));

            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private string GetIpAddress(HttpContext httpContext)
        {
            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        }
    }
}