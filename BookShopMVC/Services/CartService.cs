using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model;
using BookShopMVC.Model.ViewModels;
using BookStoreMVC.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace BookShopMVC.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CartService(IUnitOfWork unitOfWork, IServiceScopeFactory serviceScopeFactory)
        {
            _unitOfWork = unitOfWork;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public ServiceResult AddItem(int productId, int quantity, string userId)
        {
            Product product = _unitOfWork.Product.GetById(productId);
            if (product == null)
            {
                return new ServiceResult { Success = false, Message = "Sản phẩm không tồn tại." };
            }

            ShoppingCartItem itemInCart = _unitOfWork.CartItem.GetByUserId(userId)
                .FirstOrDefault(p => p.productId == productId);

            if (itemInCart != null)
            {
                itemInCart.quantity += quantity;
                _unitOfWork.CartItem.Update(itemInCart);
            }
            else
            {
                ShoppingCartItem shoppingCartItem = new()
                {
                    productId = productId,
                    quantity = quantity,
                    userId = userId
                };
                _unitOfWork.CartItem.Add(shoppingCartItem);
            }
            _unitOfWork.Save();
            return new ServiceResult { Success = true, Message = "Sản phẩm đã được thêm vào giỏ hàng." };
        }

        public ServiceResult PlaceOrder(SummaryVM summaryVM)
        {
            Order order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                UserId = summaryVM.Cart.UserId,
                Date = DateTime.Now,
                Status = Constants.OrderStatus.Pending,
                StreetAddress = summaryVM.StreetAddress,
                City = summaryVM.City,
                State = summaryVM.State,
                PostalCode = summaryVM.PostalCode,
                PhoneNumber = summaryVM.PhoneNumber,
            };

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var items = summaryVM.Cart.Items.ToList(); // Load danh sách sản phẩm trước khi vòng lặp

                foreach (var item in items)
                {
                    var product = scopedUnitOfWork.Product.GetById(item.productId);

                    order.Items.Add(new OrderItem()
                    {
                        ProductId = item.productId,
                        Quantity = item.quantity,
                        Price = product?.Price ?? 0
                    });
                }

                scopedUnitOfWork.Order.Add(order);
                scopedUnitOfWork.ShoppingCart.ClearCart(summaryVM.Cart.UserId);
                scopedUnitOfWork.Save();
            }

            return new ServiceResult { Success = true, Message = "Đơn hàng đã được đặt thành công." };
        }

        public ServiceResult UpdateQuantity(int productId, int quantity, string userId)
        {
            ShoppingCartItem shoppingCartItem = _unitOfWork.CartItem.GetByUserId(userId)
                .FirstOrDefault(p => p.productId == productId);

            if (shoppingCartItem == null)
            {
                return new ServiceResult { Success = false, Message = "Không tìm thấy sản phẩm trong giỏ hàng." };
            }

            shoppingCartItem.quantity = quantity;
            _unitOfWork.CartItem.Update(shoppingCartItem);
            _unitOfWork.Save();
            return new ServiceResult { Success = true, Message = "Đã cập nhật giỏ hàng." };
        }
    }
}
