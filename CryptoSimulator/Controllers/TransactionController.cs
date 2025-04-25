using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("api/transactions/myTransactions")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetMyTransactions()
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var transactions = await _transactionService.GetTransactionsByUserIdAsync(userId);
            return Ok(transactions);
        }

        [HttpPost("api/trade/buy")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> BuyCrypto([FromBody] TransactionCreateDto transactionDto)
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var result = await _transactionService.BuyTransactionAsync(userId, transactionDto);
            return Ok(result);
        }
        [HttpPost("api/trade/sell")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> SellCrypto([FromBody] TransactionCreateDto transactionDto)
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var result = await _transactionService.SellTransactionAsync(userId, transactionDto);
            return Ok(result);
        }

        [HttpGet("api/transactions/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTransactions(int userId)
        {
            var transactions = await _transactionService.GetTransactionsByUserIdAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("api/transactions/details/{transactionId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetTransactionDetails(int transactionId)
        {
            var transaction = await _transactionService.GetDetailedTransactionsByIdAsync(transactionId);
            return Ok(transaction);
        }
    }
}
