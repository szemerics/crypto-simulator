using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Authorize]
    public class LimitOrderController : ControllerBase
    {
        private readonly ILimitOrderService _limitOrderService;

        public LimitOrderController(ILimitOrderService limitOrderService)
        {
            _limitOrderService = limitOrderService;
        }

        [HttpPost("api/trade/limit-buy")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> CreateLimitBuyOrder([FromBody] LimitOrderCreateDto orderDto)
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            try
            {
                var result = await _limitOrderService.CreateLimitBuyOrderAsync(userId, orderDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("api/trade/limit-sell")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> CreateLimitSellOrder([FromBody] LimitOrderCreateDto orderDto)
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            try
            {
                var result = await _limitOrderService.CreateLimitSellOrderAsync(userId, orderDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("api/trade/limit-orders/")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetUserLimitOrders()
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            try
            {
                var orders = await _limitOrderService.GetLimitOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("api/trade/limit-orders/{orderId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> CancelLimitOrder(int orderId)
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            try
            {
                var result = await _limitOrderService.CancelLimitOrderAsync(orderId, userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
} 