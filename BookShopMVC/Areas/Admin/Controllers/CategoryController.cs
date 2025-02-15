using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model;
using BookShopMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name!.ToLower() == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Tên không được trùng với thứ tự hiển thị");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Danh mục đã được tạo thành công!!";
                return RedirectToAction("Index", "Category");

            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.GetById((int)id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Danh mục đã được sửa thành công!";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.GetById((int)id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Delete(Category category)
        {
            Category? categoryFromDb = _unitOfWork.Category.GetById(category.Id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(categoryFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Danh mục đã được xóa thành công!";
            return RedirectToAction("Index", "Category");
        }
    }
}
