using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model;
using Microsoft.AspNetCore.Mvc;

namespace BookShopMVC.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        IUnitOfWork _unitOfWork;

        public IViewComponentResult Invoke()
        {
            IEnumerable<Category> categoryList = _unitOfWork.Category.GetAll();
            return View(categoryList);
        }

        public FooterViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
