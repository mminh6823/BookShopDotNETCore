using BookShopMVC.DataAccess.Data;
using BookShopMVC.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookShopMVC.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext? db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
            _db.Products.Include(m => m.Category).Include(m => m.CategoryId);
            _db.UserProductShoppingCarts.Include(m => m.Product).Include(m => m.productId);
        }

        public ApplicationDbContext Db { get; }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            // Bắt đầu truy vấn từ DbSet
            IQueryable<T> query = dbSet!;

            // Áp dụng điều kiện lọc
            query = query.Where(filter);

            // Nếu có includeProperties, tách các thuộc tính và Include()
            if (!string.IsNullOrEmpty(includeProperties))
            {
                var properties = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var property in properties)
                {
                    query = query.Include(property.Trim());
                }
            }

            // Trả về bản ghi đầu tiên tìm thấy hoặc null nếu không có
            return query.FirstOrDefault();
        }


        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            // Bắt đầu truy vấn từ DbSet
            IQueryable<T> query = dbSet;

            // Nếu có includeProperties, tách các thuộc tính và Include()
            if (!string.IsNullOrEmpty(includeProperties))
            {
                var properties = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var property in properties)
                {
                    query = query.Include(property.Trim());
                }
            }

            // Trả về danh sách kết quả
            return query.ToList();
        }


        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
