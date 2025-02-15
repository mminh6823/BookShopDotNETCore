using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model.Validation
{
    internal class MaxFileSizeKbAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeKbAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {

                if (file.Length > (1024 * _maxFileSize))
                {
                    return new ValidationResult($"Kích thước tệp tối đa được phép là {_maxFileSize} KB.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
