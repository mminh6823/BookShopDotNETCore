using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model.DTO
{
    public class ProductDTO
    {
        [Required]
        public int ProductId { get; set; }
        [Range(1, 1000, ErrorMessage = "Số lượng phải nằm trong khoảng từ 1 đến 1000")]
        public int Quantity { get; set; }
    }
}
