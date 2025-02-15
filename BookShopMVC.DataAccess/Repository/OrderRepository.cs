using BookShopMVC.DataAccess.Data;
using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model;
using BookStoreMVC.Utility;
using Microsoft.EntityFrameworkCore;

namespace BookShopMVC.DataAccess.Repository
{
    internal class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }

        public IEnumerable<Order> GetAllUserOrders(string userId)
        {
            var orders = _db.Orders
               .Where(o => o.UserId == userId)
               .Include(o => o.Items)
               .ToList();

            return orders;
        }

        public Order GetOrder(int orderId)
        {
            return _db.Orders
                 .Include(o => o.Items)
                 .ThenInclude(i => i.Product)
                 .FirstOrDefault(o => o.Id == orderId);
        }

        public void Update(Order order)
        {
            _db.Orders.Update(order);
        }

        public void UpdateStatus(Order order, Constants.OrderStatus orderStatus)
        {
            order.Status = orderStatus;
            _db.Orders.Update(order);
        }
    }
}
