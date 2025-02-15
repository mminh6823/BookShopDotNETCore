using BookShopMVC.DataAccess.Data;
using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.DataAccess.Repository
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderItemRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public OrderItem? GetById(int id)
        {
            return _db.OrderItems
                 .Include(p => p.Product)
                 .FirstOrDefault(p => p.Id == id);
        }
    }
}
