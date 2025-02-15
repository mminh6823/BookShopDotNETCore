using BookShopMVC.Model.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model.ViewModels
{
    public class ProductVM
    {
        public Product? Product { get; set; }

        [MaxFileSizeKb(500)]
        [AllowedExtension(new string[] { ".jpg", "jpeg" })]
        [ImageDimensions(390, 595)]
        public IFormFile? Image { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
    }
}
