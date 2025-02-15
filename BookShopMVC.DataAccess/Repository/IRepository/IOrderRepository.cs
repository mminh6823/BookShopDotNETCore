using BookShopMVC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookStoreMVC.Utility.Constants;

namespace BookShopMVC.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        public void Update(Order order);
        public void UpdateStatus(Order order, OrderStatus orderStatus);
        public Order GetOrder(int orderId);
        IEnumerable<Order> GetAllUserOrders(string userId);
    }
}
