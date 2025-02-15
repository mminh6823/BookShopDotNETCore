using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.Model.DTO
{
    public class PriceDTO
    {
        public decimal Raw { get; }
        public string Formatted { get => Raw.ToString("c"); }

        public PriceDTO(decimal raw)
        {
            Raw = raw;
        }
    }
}
