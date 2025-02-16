using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model.ViewModels;
using BookShopMVC.Model;
using BookShopMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookShopMVC.Utility;

namespace BookShopMVC.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageService _imageService;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IImageService imageService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(products);
        }

        // GET: Product/Upsert/{id?}
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product(),
            };

            if (id == null || id == 0)
            {
                // Tạo mới
                return View(productVM);
            }
            else
            {
                // Cập nhật
                productVM.Product = _unitOfWork.Product.GetById((int)id);
                if (productVM.Product == null)
                {
                    TempData["error"] = "Không tìm thấy sản phẩm.";
                    return RedirectToAction("Index");
                }
                return View(productVM);
            }
        }

        // POST: Product/Upsert

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string productPath = Path.Combine(wwwRootPath, @"images\");
                    _imageService.DeleteIfExists(wwwRootPath, productVM.Product.ImageUrl);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = productVM.Product.Id == 0
                    ? "Thêm sản phẩm thành công"
                    : "Cập nhật sản phẩm thành công";
                return RedirectToAction("index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        #region API_CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Lỗi khi xóa" });
            }

            var productToBeDeleted = _unitOfWork.Product.GetById((int)id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Lỗi khi xóa" });
            }

            // Xóa ảnh nếu tồn tại
            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\', '/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Xóa thành công" });
        }
        #endregion
    }
}
