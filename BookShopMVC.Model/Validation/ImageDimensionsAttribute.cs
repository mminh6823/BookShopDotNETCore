using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model.Validation
{
    internal class ImageDimensionsAttribute : ValidationAttribute
    {
        private readonly int _width;
        private readonly int _height;

        public ImageDimensionsAttribute(int width, int height)
        {
            _width = width;
            _height = height;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                using var img = Image.Load(file.OpenReadStream());
                if (img.Width != _width || img.Height != _height)
                {
                    return new ValidationResult($"Kích thước của hình ảnh phải là {_width}x{_height} pixels.");
                }
            }
            return ValidationResult.Success;
        }

    }
}
