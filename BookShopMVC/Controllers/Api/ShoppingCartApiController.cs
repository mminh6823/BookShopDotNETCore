using Ajax;
using BookShopMVC.DataAccess.Repository.IRepository;
using BookShopMVC.Model.DTO;
using BookShopMVC.Model.Mappers;
using BookShopMVC.Model;
using BookShopMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookShopMVC.Utility;

namespace BookShopMVC.Controllers.Api
{
    [Route("api/user/cart")]
    [ApiController]
    [Authorize(Roles = StaticDetails.Role_Cust + "," + StaticDetails.Role_Admin)]
    public class ShoppingCartApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartApiController(IUnitOfWork unitOfWork, ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cartService = cartService;
        }

        [HttpGet]
        public ActionResult<JSend> GetCart()
        {
            string? userId = _userManager.GetUserId(HttpContext.User);
            Cart cartVM = new Cart
            {
                Items = _unitOfWork.CartItem.GetByUserId(userId!)
            };
            CartDTO cartDTO = ShoppingCartMapper.MapToDto(cartVM);
            return Ok(JSend.Success(cartDTO));
        }

        // GET api/user/cart
        [HttpGet("item/{productId}")]
        public ActionResult<JSend> GetCart(int productId)
        {
            if (_unitOfWork.Product.GetById(productId) == null)
            {
                return NotFound(JSend.Fail("Sản phẩm không tồn tại"));
            }
            string? userId = _userManager.GetUserId(HttpContext.User);
            ShoppingCartItem? shoppingCartItem = _unitOfWork.CartItem.GetByUserId(userId!)
                .FirstOrDefault(p => p.productId == productId);

            if (shoppingCartItem == null)
            {
                return NotFound(JSend.Fail("Sản phẩm không có trong giỏ hàng"));
            }

            CartItemDTO cartItemDTO = ShoppingCartMapper.MapToDto(shoppingCartItem);
            return Ok(JSend.Success(cartItemDTO));
        }

        // POST api/user/cart
        [HttpPost]
        public ActionResult<JSend> AddItemToCart([FromBody] ProductDTO newCartItem)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            JSend response;

            ServiceResult result = _cartService.AddItem(newCartItem.ProductId, newCartItem.Quantity, userId);
            if (!result.Success)
            {
                return NotFound(JSend.Fail());
            }

            Cart cartVM = new Cart
            {
                Items = _unitOfWork.CartItem.GetByUserId(userId)
            };
            CartDTO cartDTO = ShoppingCartMapper.MapToDto(cartVM);

            return Ok(JSend.Success(cartDTO));
        }

        // PUT api/user/cart/item/5
        [HttpPut("item/{id}")]
        public ActionResult<JSend> UpdateCartItem(int id, [FromBody] int quantity)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            ServiceResult result = _cartService.UpdateQuantity(id, quantity, userId);

            if (!result.Success)
            {
                return NotFound(JSend.Fail(result.Message));
            }
            Cart cartVM = new Cart
            {
                Items = _unitOfWork.CartItem.GetByUserId(userId)
            };
            CartDTO cartDTO = ShoppingCartMapper.MapToDto(cartVM);
            return Ok(JSend.Success(cartDTO));
        }

        // DELETE api/user/cart/item/5
        [HttpDelete("item/{id}")]
        public ActionResult<JSend> DeleteCartItem(int id)
        {
            Product? product = _unitOfWork.Product.GetById(id);

            if (product == null)
            {
                return NotFound(JSend.Fail("Sản phẩm không tồn tại"));
            }

            string? userId = _userManager.GetUserId(HttpContext.User);
            ShoppingCartItem? shoppingCartItem = _unitOfWork.CartItem.GetByUserId(userId)
                .FirstOrDefault(p => p.productId == id);

            if (shoppingCartItem == null)
            {
                return NotFound(JSend.Fail("Sản phẩm không có trong giỏ hàng"));
            }

            _unitOfWork.CartItem.Remove(shoppingCartItem);
            _unitOfWork.Save();

            Cart cartVM = new Cart
            {
                Items = _unitOfWork.CartItem.GetByUserId(userId)
            };
            CartDTO cartDTO = ShoppingCartMapper.MapToDto(cartVM);
            return Ok(JSend.Success(cartDTO));
        }
    }
}
