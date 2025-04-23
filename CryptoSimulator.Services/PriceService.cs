using AutoMapper;
using CryptoSimulator.DataContext.Context;
using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.Services
{
    public interface IPriceService
    {
        Task<CryptoCurrencyDto> SetPriceAsync(int cryptoId, decimal newPrice);
        Task<IEnumerable<PriceHistoryDto>> GetPriceHistoryAsync(int cryptoId);
    }
    
    public class PriceService : IPriceService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public PriceService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PriceHistoryDto>> GetPriceHistoryAsync(int cryptoId)
        {
            var crypto = await _context.FindAsync<CryptoCurrency>(cryptoId);

            if (crypto == null)
            {
                throw new ArgumentNullException("Crypto currency not found");
            }

            var priceHistory = await _context.PriceHistories
                .Where(ph => ph.CryptoCurrencyId == cryptoId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PriceHistoryDto>>(priceHistory);
        }

        public async Task<CryptoCurrencyDto> SetPriceAsync(int cryptoId, decimal newPrice)
        {
            var crypto = await _context.FindAsync<CryptoCurrency>(cryptoId);

            if (crypto == null)
            {
                throw new ArgumentNullException("Crypto currency not found");
            }

            crypto.CurrentPrice = newPrice;
            _context.CryptoCurrencies.Update(crypto);
            await _context.SaveChangesAsync();

            return _mapper.Map<CryptoCurrencyDto>(crypto);
        }
    }
}
