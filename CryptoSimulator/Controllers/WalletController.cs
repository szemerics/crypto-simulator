using CryptoSimulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Route("api/wallets/[action]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletByUserId(int userId)
        {
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            return Ok(wallet);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWalletBalanceByUserId(int userId, int balance)
        {
            var wallet = await _walletService.UpdateWalletBalanceByUserIdAsync(userId, balance);
            return Ok(wallet);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteWalletByUserId(int userId)
        {
            var result = await _walletService.DeleteWalletByUserIdAsync(userId);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
