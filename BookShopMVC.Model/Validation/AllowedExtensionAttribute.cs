using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model.Validation
{
    internal class AllowedExtensionAttribute : ValidationAttribute
    {
        private string[] _extensions;

        public AllowedExtensionAttribute(string[] extensions)
        {
            _extensions = extensions;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions!.Contains(extension.ToLower()))
                {
                    return new ValidationResult($"Chúng tôi chỉ chấp nhận hình ảnh có những định dạng tệp tin sau: {String.Join(", ", _extensions!)}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
