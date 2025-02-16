using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model.ViewModels;
using BookShopMVC.Model;
using BookShopMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookShopMVC.Utility;

namespace BookShopMVC.Controllers
{
    [Authorize(Roles = StaticDetails.Role_Cust + "," + StaticDetails.Role_Admin)]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService;
        public ShoppingCartController(IUnitOfWork unitOfWork, ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cartService = cartService;
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

            if (summaryVM.RememberAddress)
            {
                user.State = summaryVM.State;
                user.City = summaryVM.City;
                user.StreetAddress = summaryVM.StreetAddress;
                user.PostalCode = summaryVM.PostalCode;
                user.PhoneNumber = summaryVM.PhoneNumber;
                _userManager.UpdateAsync(user);
            }

            summaryVM.Cart = cart;
            ServiceResult result = _cartService.PlaceOrder(summaryVM);
            if (result.Success == false)
            {
                TempData["error"] = "Lỗi khi đặt hàng";
                return RedirectToAction("Summary");
            }

            TempData["success"] = "Đơn hàng đã được đặt thành công!";
            return RedirectToAction("Index", "Home");
        }
    }
}
