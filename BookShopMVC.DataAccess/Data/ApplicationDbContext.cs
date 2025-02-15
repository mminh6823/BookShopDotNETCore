using BookShopMVC.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopMVC.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCartItem> UserProductShoppingCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var deletedBooks = ChangeTracker.Entries<Product>()
                .Where(e => e.State == EntityState.Deleted);

            foreach (var bookEntry in deletedBooks)
            {
                var book = bookEntry.Entity;
                var imagePath = book.ImageUrl;
                if (string.IsNullOrEmpty(imagePath))
                {
                    continue;
                }
                DeleteBookImage(imagePath);
            }

            return await base.SaveChangesAsync();

        }

        public override int SaveChanges()
        {
            var deletedBooks = ChangeTracker.Entries<Product>()
                .Where(e => e.State == EntityState.Deleted);

            foreach (var bookEntry in deletedBooks)
            {
                var book = bookEntry.Entity;
                var imagePath = book.ImageUrl;
                if (string.IsNullOrEmpty(imagePath))
                {
                    continue;
                }
                DeleteBookImage(imagePath);
            }

            return base.SaveChanges();
        }

        private void DeleteBookImage(string? imagePath)
        {

            if (!File.Exists(imagePath))
            {
                return;
            }
            File.Delete(imagePath);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Hành động", DisplayOrder = 3 },
                new Category { Id = 2, Name = "Kịch tính", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Kinh dị", DisplayOrder = 1 },
                new Category { Id = 4, Name = "Khoa học viễn tưởng", DisplayOrder = 4 }
                );

            modelBuilder.Entity<OrderItem>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Items)
                .WithOne(e => e.Order)
                .HasForeignKey(e => e.OrderId)
                .IsRequired();

            modelBuilder.Entity<Product>()
               .HasOne(p => p.Category)
               .WithMany()
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Huyền Thoại Rồng Lửa",
                    Author = "Nguyễn Nhật Ánh",
                    Description = "Cuốn tiểu thuyết hành động với những pha giao tranh nghẹt thở giữa các nhân vật mạnh mẽ trong bối cảnh lịch sử Việt Nam.",
                    ISBN = "VN0001001",
                    Price = 120,
                    Price50 = 110,
                    Price100 = 100,
                    CategoryId = 1, // Hành động
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 2,
                    Title = "Mê Cung Tình Yêu",
                    Author = "Trần Anh Tuấn",
                    Description = "Một tác phẩm kịch tính xoay quanh những mối quan hệ phức tạp và những bí mật gia đình không thể giấu được.",
                    ISBN = "VN0001002",
                    Price = 100,
                    Price50 = 95,
                    Price100 = 90,
                    CategoryId = 2, // Kịch tính
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "Bóng Ma Trong Đêm",
                    Author = "Phạm Minh Đức",
                    Description = "Cuốn sách kinh dị mang đến những tràng đêm rùng rợn với những hiện tượng siêu nhiên và bí ẩn chưa lời giải.",
                    ISBN = "VN0001003",
                    Price = 110,
                    Price50 = 105,
                    Price100 = 100,
                    CategoryId = 3, // Kinh dị
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 4,
                    Title = "Vũ Trụ Huyền Bí",
                    Author = "Lê Hoàng Nam",
                    Description = "Một hành trình xuyên qua không gian và thời gian, khám phá những bí mật của vũ trụ trong cuốn tiểu thuyết khoa học viễn tưởng.",
                    ISBN = "VN0001004",
                    Price = 130,
                    Price50 = 125,
                    Price100 = 120,
                    CategoryId = 4, // Khoa học viễn tưởng
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 5,
                    Title = "Sắc Màu Đời Thường",
                    Author = "Ngô Thị Mai",
                    Description = "Tác phẩm văn học đặc sắc khắc họa cuộc sống thường nhật qua lăng kính tinh tế và đầy cảm hứng.",
                    ISBN = "VN0001005",
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 2, // Kịch tính
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 6,
                    Title = "Hành Trình Tương Lai",
                    Author = "Vũ Minh Khang",
                    Description = "Cuốn tiểu thuyết khoa học viễn tưởng đưa người đọc vào một thế giới tương lai với công nghệ tiên tiến và những thách thức không ngờ.",
                    ISBN = "VN0001006",
                    Price = 140,
                    Price50 = 135,
                    Price100 = 130,
                    CategoryId = 4, // Khoa học viễn tưởng
                    ImageUrl = ""
                }
              );
        }
    }
}
