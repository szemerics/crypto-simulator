using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Route("api/cryptos")]
    [Authorize]
    public class CryptoCurrencyController : ControllerBase
    {
        private readonly ICryptoCurrencyService _cryptoCurrencyService;

        public CryptoCurrencyController(ICryptoCurrencyService cryptoCurrencyService)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCryptoCurrencies()
        {
            var cryptoCurrencies = await _cryptoCurrencyService.GetAllCryptoCurrenciesAsync();
            return Ok(cryptoCurrencies);
        }

        [HttpGet("{cryptoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCryptoById(int cryptoId)
        {
            var crypto = await _cryptoCurrencyService.GetCryptoCurrencyByIdAsync(cryptoId);

            return Ok(crypto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCrypto([FromBody] CryptoCurrencyCreateDto cryptoCurrencyDto)
        {
            var crypto = await _cryptoCurrencyService.CreateCryptoCurrencyAsync(cryptoCurrencyDto);

            return CreatedAtAction(nameof(GetCryptoById), new { cryptoId = crypto.Id }, crypto);
        }

        [HttpDelete("{cryptoId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCrypto(int cryptoId)
        {
            var result = await _cryptoCurrencyService.DeleteCryptoCurrencyAsync(cryptoId);

            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
