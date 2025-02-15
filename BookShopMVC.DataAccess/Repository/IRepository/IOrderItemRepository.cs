using BookShopMVC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.DataAccess.Repository.IRepository
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        public OrderItem? GetById(int id);
    }
}
