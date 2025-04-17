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
    public interface ICryptoCurrencyService
    {
        Task<IEnumerable<CryptoCurrencyDto>> GetAllCryptoCurrenciesAsync();
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
    }
}
