using CryptoSimulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Route("api/prices/[action]")]
    public class PriceController : ControllerBase
    {
        private readonly IPriceService _priceService;

        public PriceController(IPriceService priceService)
        {
            _priceService = priceService;
        }

        [HttpGet("{cryptoId}")]
        public async Task<IActionResult> GetPriceHistory(int cryptoId)
        {
            var priceHistory = await _priceService.GetPriceHistoryAsync(cryptoId);
            return Ok(priceHistory);
        }

        [HttpPut]
        public async Task<IActionResult> SetPrice(int cryptoId, decimal newPrice)
        {
            var updatedCrypto = await _priceService.SetPriceAsync(cryptoId, newPrice);
            return Ok(updatedCrypto);
        }

    }
}
