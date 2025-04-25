using CryptoSimulator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("myWallet")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetMyWallet()
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            return Ok(wallet);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetWalletByUserId(int userId)
        {
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            return Ok(wallet);
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateWalletBalanceByUserId(int userId, int balance)
        {
            var wallet = await _walletService.UpdateWalletBalanceByUserIdAsync(userId, balance);
            return Ok(wallet);
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
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
