using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public string? ISBN { get; set; }
        [Required]
        public string? Author { get; set; }

        [Required]
        [Display(Name = "Giá từ 1 - 50")]
        [Range(1, 1000)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Giá từ 50+")]
        [Range(1, 1000)]
        public decimal Price50 { get; set; }

        [Required]
        [Display(Name = "Giá từ 100+")]
        [Range(1, 1000)]
        public decimal Price100 { get; set; }

        public int? CategoryId { get; set; }
        [ValidateNever]
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        private string? imageUrl;
        [ValidateNever]
        public string? ImageUrl
        {
            get => string.IsNullOrEmpty(imageUrl) ? "/images/product/no_photo.jpg" : imageUrl;
            set => imageUrl = value;
        }

        [Display(Name = "Mới")]
        public bool IsNew { get; set; }
        [Display(Name = "Bán chạy")]
        public bool IsBestseller { get; set; }
        [Display(Name = "Ưu đãi đặc biệt")]
        public bool IsSpecialOffer { get; set; }

    }
}
