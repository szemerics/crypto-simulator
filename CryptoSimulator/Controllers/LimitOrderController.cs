using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Route("api/trade")]
    public class LimitOrderController : ControllerBase
    {
        private readonly ILimitOrderService _limitOrderService;

        public LimitOrderController(ILimitOrderService limitOrderService)
        {
            _limitOrderService = limitOrderService;
        }

        [HttpPost("limit-buy")]
        public async Task<IActionResult> CreateBuyLimitOrderAsync(int userId, [FromBody] LimitOrderCreateDto limitOrderDto)
        {
            var result = await _limitOrderService.CreateBuyLimitOrderAsync(userId, limitOrderDto);
            return Ok(result);
        }

        [HttpPost("limit-sell")]
        public async Task<IActionResult> CreateSellLimitOrderAsync(int userId, [FromBody] LimitOrderCreateDto limitOrderDto)
        {
            var result = await _limitOrderService.CreateSellLimitOrderAsync(userId, limitOrderDto);
            return Ok(result);
        }

        [HttpGet("limit-orders/{userId}")]
        public async Task<IActionResult> GetLimitOrdersByUserIdAsync(int userId)
        {
            var result = await _limitOrderService.GetLimitOrdersByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpDelete("limit-orders/{orderId}")]
        public async Task<IActionResult> DeleteLimitOrderAsync(int orderId)
        {
            var result = await _limitOrderService.DeleteLimitOrderAsync(orderId);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
