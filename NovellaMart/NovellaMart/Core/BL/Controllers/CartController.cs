using Microsoft.AspNetCore.Mvc;
using NovellaMart.Core.BL.Services; // Needs to see the Service
using NovellaMart.Core.BL.Model_Classes;

namespace NovellaMart.Core.BL.Controllers // NEW NAMESPACE
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            return Ok(_cartService.GetCart());
        }
    }
}