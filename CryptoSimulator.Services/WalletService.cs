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
    public interface IWalletService
    {
        Task<WalletDto> GetWalletByUserIdAsync(int userId);
        Task<WalletDto> UpdateWalletBalanceByUserIdAsync(int userId, int balance);
        Task<bool> DeleteWalletByUserIdAsync(int userId);
    }

    public class WalletService : IWalletService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;
        public WalletService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<WalletDto> GetWalletByUserIdAsync(int userId)
        {
            var userWallet = await _context.Wallets
                        .Include(w => w.User)
                        .Include(w => w.Portfolios)
                            .ThenInclude(p => p.CryptoCurrency)
                            //.IgnoreQueryFilters() // -> if you want to include soft-deleted portfolios
                        .FirstOrDefaultAsync(w => w.UserId == userId);

            if (userWallet == null)
            {
                throw new ArgumentNullException("Wallet not found");
            }

            return _mapper.Map<WalletDto>(userWallet);
        }

        public async Task<WalletDto> UpdateWalletBalanceByUserIdAsync(int userId, int balance)
        {
            var userWallet = await _context.Wallets
            .Include(w => w.User)
            .Include(w => w.Portfolios)
                .ThenInclude(p => p.CryptoCurrency)
            .FirstOrDefaultAsync(w => w.UserId == userId);

            if (userWallet == null)
            {
                throw new ArgumentNullException("Wallet not found");
            }

            userWallet.Balance = balance;
            _context.Wallets.Update(userWallet);
            await _context.SaveChangesAsync();

            return _mapper.Map<WalletDto>(userWallet);
        }

        public async Task<bool> DeleteWalletByUserIdAsync(int userId)
        {
            var userWallet = await _context.Wallets
                .Include(w => w.User)
                .Include(w => w.Portfolios)
                    .ThenInclude(p => p.CryptoCurrency)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (userWallet == null)
            {
                throw new ArgumentNullException("Wallet not found");
            }

            //// soft-delete
            //userWallet.isDeleted = true;
            //_context.Wallets.Update(userWallet);

            userWallet.Portfolios.Clear(); // Clearing portfolios
            userWallet.Balance = 10000; // Resetting balance to default value

            _context.Wallets.Update(userWallet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
