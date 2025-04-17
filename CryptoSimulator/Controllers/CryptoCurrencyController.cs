using CryptoSimulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    public class CryptoCurrencyController : ControllerBase
    {
        private readonly ICryptoCurrencyService _cryptoCurrencyService;

        public CryptoCurrencyController(ICryptoCurrencyService cryptoCurrencyService)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
        }



        [HttpGet("api/cryptos/[action]")]
        public async Task<IActionResult> GetAllCryptoCurrencies()
        {
            var cryptoCurrencies = await _cryptoCurrencyService.GetAllCryptoCurrenciesAsync();
            return Ok(cryptoCurrencies);
        }
    }
}
