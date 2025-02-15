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
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _db;

        public CartRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }

        public void ClearCart(string userId)
        {
            IEnumerable<ShoppingCartItem> items = _db.UserProductShoppingCarts.Where(u => u.userId == userId);
            _db.UserProductShoppingCarts.RemoveRange(items);
        }

        public Cart GetCart(string userId)
        {

            IEnumerable<ShoppingCartItem> items = _db.UserProductShoppingCarts.Where(u => u.userId == userId)
                .Include(u => u.Product)
                .Where(u => u.userId == userId);

            Cart cart = new Cart()
            {
                Items = items,
                UserId = userId
            };
            return cart;
        }
    }
}
