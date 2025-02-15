using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model;
using BookShopMVC.Model.ViewModels;
using BookStoreMVC.Utility;

namespace BookShopMVC.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResult AddItem(int productId, int quantity, string userId)
        {
            Product product = _unitOfWork!.Product!.GetById(productId)!;
            if (product == null)
            {
                return new ServiceResult() { Success = false, Message = "Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng." };
            }

            ShoppingCartItem? itemInCart = _unitOfWork.CartItem.GetByUserId(userId)
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
            return new ServiceResult() { Success = true, Message = "Sản phẩm đã được thêm vào giỏ hàng." };
        }

        public ServiceResult PlaceOrder(SummaryVM summaryVM)
        {
            Order order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                UserId = summaryVM.Cart!.UserId,
                Date = DateTime.Now,
                Status = Constants.OrderStatus.Pending,
                StreetAddress = summaryVM.StreetAddress,
                City = summaryVM.City,
                State = summaryVM.State,
                PostalCode = summaryVM.PostalCode,
                PhoneNumber = summaryVM.PhoneNumber,
            };

            foreach (var item in summaryVM.Cart!.Items!)
            {
                order.Items.Add(new OrderItem()
                {
                    ProductId = item.productId,
                    Quantity = item.quantity,
                    Price = item.Product!.Price
                });
            }

            _unitOfWork.Order.Add(order);
            _unitOfWork.ShoppingCart.ClearCart(summaryVM.Cart.UserId!);
            _unitOfWork.Save();
            return new ServiceResult() { Success = true, Message = "Đơn hàng đã được đặt thành công." };
        }

        public ServiceResult UpdateQuantity(int productId, int quantity, string? userId)
        {
            ShoppingCartItem shoppingCartItem = _unitOfWork!.CartItem!.GetByUserId(userId!)
                .FirstOrDefault(p => p.productId == productId)!;

            if (shoppingCartItem == null)
            {
                return new ServiceResult() { Success = false, Message = "Không tìm thấy sản phẩm trong giỏ hàng." };
            }

            shoppingCartItem.quantity = quantity;
            _unitOfWork.CartItem.Update(shoppingCartItem);
            _unitOfWork.Save();
            return new ServiceResult() { Success = true, Message = "Đã cập nhật giỏ hàng!!!!!" };
        }
    }
}
