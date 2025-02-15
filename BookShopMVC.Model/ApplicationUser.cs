using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? Name { get; set; }
        [Display(Name = "Địa chỉ đường")]
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Display(Name = "Mã bưu điện")]
        public string? PostalCode { get; set; }
        [Display(Name = "Số điện thoại")]
        override public string? PhoneNumber { get; set; }
    }
}
