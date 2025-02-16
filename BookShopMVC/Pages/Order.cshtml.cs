using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model;
using BookShopMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookShopMVC.Areas.Admin.Pages
{
    [Authorize(Roles = StaticDetails.Role_Cust + "," + StaticDetails.Role_Admin)]
    public class OrderModel : PageModel
    {
        public Order order;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderModel(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult OnGet(int id)
        {
            string userId = _userManager.GetUserId(User);
            order = _unitOfWork.Order.GetOrder(id);
            if (order == null || order.UserId != userId)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }
    }
}
