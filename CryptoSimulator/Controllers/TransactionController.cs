using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Route("api/trade")]
    [Authorize(Roles = "Admin, User")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyCrypto([FromBody] TransactionCreateDto transactionDto)
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var result = await _transactionService.BuyTransactionAsync(userId, transactionDto);
            return Ok(result);
        }
        [HttpPost("sell")]
        public async Task<IActionResult> SellCrypto([FromBody] TransactionCreateDto transactionDto)
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var result = await _transactionService.SellTransactionAsync(userId, transactionDto);
            return Ok(result);
        }
    }
}
