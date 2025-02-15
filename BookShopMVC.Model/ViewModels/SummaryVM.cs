using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model.ViewModels
{
    public class SummaryVM
    {
        public Cart? Cart { get; set; }
        [Display(Name = "Địa chỉ đường")]
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Display(Name = "Mã bưu điệ")]
        public string? PostalCode { get; set; }
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }
        public bool RememberAddress { get; set; }
    }
}
