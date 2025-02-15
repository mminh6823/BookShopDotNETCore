using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [Display(Name = "Tên thể loại")]
        public string? Name { get; set; }
        [Display(Name = "Thứ tự hiển thị")]
        [Range(1, 100, ErrorMessage = "Thứ tự hiển thị phải nằm trong khoảng từ 1 đến 100.")]
        public int DisplayOrder { get; set; }
    }
}
