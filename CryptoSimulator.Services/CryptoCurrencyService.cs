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
    public interface ICryptoCurrencyService
    {
        Task<IEnumerable<CryptoCurrencyDto>> GetAllCryptoCurrenciesAsync();
        Task<CryptoCurrencyDto> GetCryptoCurrencyByIdAsync(int id);
        Task<CryptoCurrencyDto> CreateCryptoCurrencyAsync(CryptoCurrencyCreateDto cryptoCurrencyDto);
        Task<bool> DeleteCryptoCurrencyAsync(int id);
    }

    public class CryptoCurrencyService : ICryptoCurrencyService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public CryptoCurrencyService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CryptoCurrencyDto>> GetAllCryptoCurrenciesAsync()
        {
            var cryptoCurrencies = await _context.CryptoCurrencies.ToListAsync();

            return _mapper.Map<IEnumerable<CryptoCurrencyDto>>(cryptoCurrencies);
        }

        public async Task<CryptoCurrencyDto> GetCryptoCurrencyByIdAsync(int id)
        {
            var cryptoCurrency = await _context.CryptoCurrencies.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (cryptoCurrency == null)
            {
                throw new ArgumentNullException("Crypto currency not found");
            }

            return _mapper.Map<CryptoCurrencyDto>(cryptoCurrency);
        }

        public async Task<CryptoCurrencyDto> CreateCryptoCurrencyAsync(CryptoCurrencyCreateDto cryptoCurrencyDto)
        {
            var crypto = _mapper.Map<CryptoCurrency>(cryptoCurrencyDto);

            await _context.CryptoCurrencies.AddAsync(crypto);
            await _context.SaveChangesAsync();

            return _mapper.Map<CryptoCurrencyDto>(crypto);
        }

        public async Task<bool> DeleteCryptoCurrencyAsync(int id)
        {
            var cryptoCurrency = await _context.CryptoCurrencies.FindAsync(id);

            if (cryptoCurrency == null)
            {
                throw new ArgumentNullException("Crypto currency not found");
            }

            // cryptoCurrency.isDeleted = true;
            // _context.CryptoCurrencies.Update(cryptoCurrency);

            _context.CryptoCurrencies.Remove(cryptoCurrency);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
