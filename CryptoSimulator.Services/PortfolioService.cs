using AutoMapper;
using CryptoSimulator.DataContext.Context;
using CryptoSimulator.DataContext.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.Services
{
    public interface IPortfolioService
    {
        Task<List<PortfolioWalletDto>> GetPortfolioByUserIdAsync(int userId);

        Task<PortfolioProfitSummaryDto> CalculateDetailedProfitOfPortfolioAsync(int userId);
        Task<PortfolioTotalProfitDto> CalculateProfitOfPortfolioAsync(int userId);
    }
    public class PortfolioService : IPortfolioService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public PortfolioService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PortfolioProfitSummaryDto> CalculateDetailedProfitOfPortfolioAsync(int userId)
        {
            var portfolios = await _context.Portfolios
                .Where(p => p.Wallet.UserId == userId)
                .Include(p => p.CryptoCurrency)
                .ToListAsync();

            var result = new List<PortfolioProfitDto>();
            decimal totalProfit = 0;


            foreach (var portfolio in portfolios)
            {
                var currentPrice = portfolio.CryptoCurrency.CurrentPrice;
                var purchasePrice = portfolio.PurchasePrice;
                var quantity = portfolio.Quantity;

                var profit = (currentPrice - purchasePrice) * quantity;

                result.Add(new PortfolioProfitDto
                {
                    CryptoCurrencySymbol = portfolio.CryptoCurrency.Symbol,
                    Quantity = quantity,
                    Profit = profit
                });

                totalProfit += profit;
            }

            return new PortfolioProfitSummaryDto
            {
                TotalProfit = totalProfit,
                Profits = result
            };

        }

        public async Task<PortfolioTotalProfitDto> CalculateProfitOfPortfolioAsync(int userId)
        {
            var portfolios = await _context.Portfolios
                .Where(p => p.Wallet.UserId == userId)
                .Include(p => p.CryptoCurrency)
                .ToListAsync();

            decimal totalProfit = portfolios
                .Sum(p => (p.CryptoCurrency.CurrentPrice - p.PurchasePrice) * p.Quantity);

            return new PortfolioTotalProfitDto
            {
                TotalProfit = totalProfit,
            };
        }

        public async Task<List<PortfolioWalletDto>> GetPortfolioByUserIdAsync(int userId)
        {
            var portfolios = await _context.Portfolios
                .Where(p => p.Wallet.UserId == userId)
                .Include(p => p.CryptoCurrency)
                .ToListAsync();

            return _mapper.Map<List<PortfolioWalletDto>>(portfolios);
        }

    }
}
