using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product>? ProductList { get; set; }
        public IEnumerable<Category>? CategoryList { get; set; }
    }
}
