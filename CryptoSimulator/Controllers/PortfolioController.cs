using CryptoSimulator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("api/portfolio/myPortfolio")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetMyPortfolio()
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var portfolio = await _portfolioService.GetPortfolioByUserIdAsync(userId);
            return Ok(portfolio);
        }

        [HttpGet("api/profit/myProfit")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetMyProfit()
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var portfolio = await _portfolioService.CalculateProfitOfPortfolioAsync(userId);
            return Ok(portfolio);
        }

        [HttpGet("api/profit/myDetailedProfit")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetMyDetailedProfit()
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var portfolio = await _portfolioService.CalculateDetailedProfitOfPortfolioAsync(userId);
            return Ok(portfolio);
        }

        [HttpGet("api/portfolio/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPortfolioByUserId(int userId)
        {
            var portfolio = await _portfolioService.GetPortfolioByUserIdAsync(userId);
            return Ok(portfolio);
        }


        [HttpGet("api/profit/{userId}")]
        [Authorize(Roles = "Admin")]
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
