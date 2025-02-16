using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model;
using BookShopMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookShopMVC.Pages
{
    [Authorize(Roles = StaticDetails.Role_Cust + "," + StaticDetails.Role_Admin)]
    public class YourOderModel : PageModel
    {
        public IEnumerable<Order> orders ;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public YourOderModel(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public void OnGet()
        {
            string userId = _userManager.GetUserId(User);
            orders = _unitOfWork.Order.GetAllUserOrders(userId);
        }
    }
}
