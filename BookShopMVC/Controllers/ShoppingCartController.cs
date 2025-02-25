using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model.ViewModels;
using BookShopMVC.Model;
using BookShopMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookShopMVC.Utility;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BookShopMVC.Controllers
{
    [Authorize(Roles = StaticDetails.Role_Cust + "," + StaticDetails.Role_Admin)]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService;
        private readonly VNPayService _vnPayService;

        public ShoppingCartController(
            IUnitOfWork unitOfWork,
            ICartService cartService,
            UserManager<ApplicationUser> userManager,
            VNPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cartService = cartService;
            _vnPayService = vnPayService;
        }

        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User);
            Cart cart = _unitOfWork.ShoppingCart.GetCart(userId);
            return View(cart);
        }

        public IActionResult Summary()
        {
            string userId = _userManager.GetUserId(User);
            Cart cart = _unitOfWork.ShoppingCart.GetCart(userId);
            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            SummaryVM summaryVM = new SummaryVM()
            {
                Cart = cart,
                StreetAddress = user.StreetAddress ?? "",
                City = user.City ?? "",
                State = user.State ?? "",
                PostalCode = user.PostalCode ?? "",
                PhoneNumber = user.PhoneNumber ?? ""
            };
            return View(summaryVM);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(SummaryVM summaryVM)
        {
            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            Cart cart = _unitOfWork.ShoppingCart.GetCart(user.Id);

            if (cart == null || cart.Total <= 0)
            {
                TempData["error"] = "Giỏ hàng trống, không thể đặt hàng!";
                return RedirectToAction("Summary");
            }

            if (summaryVM.RememberAddress)
            {
                user.State = summaryVM.State;
                user.City = summaryVM.City;
                user.StreetAddress = summaryVM.StreetAddress;
                user.PostalCode = summaryVM.PostalCode;
                user.PhoneNumber = summaryVM.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }

            summaryVM.Cart = cart;
            ServiceResult result = _cartService.PlaceOrder(summaryVM);
            if (!result.Success)
            {
                TempData["error"] = "Lỗi khi đặt hàng";
                return RedirectToAction("Summary");
            }

            TempData["success"] = "Đơn hàng đã được đặt thành công!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult VNPayCheckout(SummaryVM summaryVM)
        {
            string userId = _userManager.GetUserId(User);
            Cart cart = _unitOfWork.ShoppingCart.GetCart(userId);

            if (cart == null || cart.Total <= 0)
            {
                TempData["error"] = "Giỏ hàng trống, không thể thanh toán.";
                return RedirectToAction("Index");
            }

            decimal totalAmount = cart.Total;
            long amountInVNPayFormat = Convert.ToInt64(totalAmount * 100);

            string orderId = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            HttpContext.Session.SetString("PendingOrderId", orderId);
            HttpContext.Session.SetString("PendingUserId", userId);

            string paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, totalAmount, orderId, "Checkout");
            return Redirect(paymentUrl);
        }

        public IActionResult PaymentReturn()
        {
            var vnpayData = Request.Query;
            string vnp_ResponseCode = vnpayData["vnp_ResponseCode"];
            string pendingOrderId = HttpContext.Session.GetString("PendingOrderId");
            string userId = HttpContext.Session.GetString("PendingUserId");

            if (string.IsNullOrEmpty(pendingOrderId) || string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Summary");
            }

            Cart cart = _unitOfWork.ShoppingCart.GetCart(userId);

            if (vnp_ResponseCode == "00") // Thành công
            {
                if (cart != null)
                {
                    SummaryVM summaryVM = new SummaryVM() { Cart = cart };
                    ServiceResult result = _cartService.PlaceOrder(summaryVM);

                    if (result.Success)
                    {
                        _unitOfWork.ShoppingCart.ClearCart(userId);
                        HttpContext.Session.Remove("PendingOrderId");
                        HttpContext.Session.Remove("PendingUserId");

                        TempData["success"] = "Thanh toán thành công! Đơn hàng của bạn đã được đặt.";
                        return RedirectToAction("Index", "Home");
                    }
                }
                TempData["error"] = "Thanh toán thành công nhưng không thể tạo đơn hàng!";
                return RedirectToAction("Summary");
            }
            else
            {
                TempData["error"] = "Thanh toán thất bại hoặc bị hủy!";
                return RedirectToAction("Summary");
            }
        }
    }
}
