using CryptoSimulator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Authorize]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet("api/portfolio/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMyPortfolio(int userId)
        {
            var portfolio = await _portfolioService.GetPortfolioByUserIdAsync(userId);
            return Ok(portfolio);
        }

        [HttpGet("api/profit/{userId}")]
        public async Task<IActionResult> GetProfitByUserId(int userId)
        {
            var portfolio = await _portfolioService.CalculateProfitOfPortfolioAsync(userId);
            return Ok(portfolio);
        }

        [HttpGet("api/profit/details/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDetailedProfitByUserId(int userId)
        {
            var portfolio = await _portfolioService.CalculateDetailedProfitOfPortfolioAsync(userId);
            return Ok(portfolio);
        }


    }
}
