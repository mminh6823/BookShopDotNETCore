using Microsoft.AspNetCore.Mvc;

namespace BookShopMVC.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode.HasValue)
            {
                if (statusCode == 404)
                {
                    return View("NotFound");
                }
                else
                {
                    return View("GenError");
                }

            }
            return View();
        }
    }
}
