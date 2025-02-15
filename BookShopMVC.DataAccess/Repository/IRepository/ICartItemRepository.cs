using BookShopMVC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.DataAccess.Repository.IRepository
{
    public interface ICartItemRepository :IRepository<ShoppingCartItem>
    {
        void Update(ShoppingCartItem userProductShoppingCart);
        public ShoppingCartItem? GetById(int id);
        public IEnumerable<ShoppingCartItem> GetByUserId(string userId);
        public IEnumerable<Product> GetUserProducts(string userId);
        public int GetShoppingCartProductsAmount(string userId);
    }
}
