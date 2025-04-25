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
