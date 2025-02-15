using System.Diagnostics;
using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model;
using Microsoft.AspNetCore.Mvc;

namespace BookShopMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }

        public IActionResult Category(int id)
        {
            Category category = _unitOfWork!.Category!.GetById(id)!;
            if (category == null)
            {
                return NotFound();
            }
            string categoryname = category.Name!;
            ViewData["CategoryName"] = categoryname;

            IEnumerable<Product> productList = _unitOfWork.Product.GetAll().Where(product => product.CategoryId == id);
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category");
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
