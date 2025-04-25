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
    public interface ITransactionService
    {
        Task<TransactionDto> BuyTransactionAsync(int userId, TransactionCreateDto transactionDto);
        Task<TransactionDto> SellTransactionAsync(int userId, TransactionCreateDto transactionDto);
    }
    public class TransactionService : ITransactionService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public TransactionService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TransactionDto> BuyTransactionAsync(int userId, TransactionCreateDto transactionDto)
        {
            var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var transaction = _mapper.Map<Transaction>(transactionDto);

                transaction.UserId = userId;

                var user = await _context.Users
                    .Include(u => u.Wallet)
                    .ThenInclude(w => w.Portfolios)
                    .ThenInclude(p => p.CryptoCurrency)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var crypto = await _context.CryptoCurrencies.Where(c => c.Id == transactionDto.CryptoCurrencyId).FirstOrDefaultAsync();

                if (crypto == null)
                {
                    throw new ArgumentNullException("Crypto not found");
                }

                if (user.Wallet.Balance < transactionDto.Quantity * crypto.CurrentPrice)
                {
                    throw new ArgumentException("Insufficient balance");
                }

                transaction.TransactionDate = DateTime.UtcNow;
                transaction.TransactionType = TransactionType.Buy;
                transaction.Price = crypto.CurrentPrice * transactionDto.Quantity;
                user.Wallet.Balance -= transactionDto.Quantity * crypto.CurrentPrice;

                var userPortfolioOfThisCrypto = user.Wallet.Portfolios.FirstOrDefault(p => p.CryptoCurrencyId == transactionDto.CryptoCurrencyId);

                if (userPortfolioOfThisCrypto == null)
                {
                    user.Wallet.Portfolios.Add(new Portfolio()
                    {
                        WalletId = user.Wallet.Id,
                        Wallet = user.Wallet,

                        CryptoCurrencyId = transactionDto.CryptoCurrencyId,
                        CryptoCurrency = crypto,
                        Quantity = transactionDto.Quantity,
                        PurchasePrice = crypto.CurrentPrice

                    });
                }
                else
                {
                    userPortfolioOfThisCrypto.Quantity += transactionDto.Quantity;
                }


                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return _mapper.Map<TransactionDto>(transaction);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception("Transaction failed - Error buying crypto", ex);
            }
        }

        public async Task<TransactionDto> SellTransactionAsync(int userId, TransactionCreateDto transactionDto)
        {
            var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var transaction = _mapper.Map<Transaction>(transactionDto);

                transaction.UserId = userId;

                var user = await _context.Users
                    .Include(u => u.Wallet)
                    .ThenInclude(w => w.Portfolios)
                    .ThenInclude(p => p.CryptoCurrency)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var crypto = await _context.CryptoCurrencies.Where(c => c.Id == transactionDto.CryptoCurrencyId).FirstOrDefaultAsync();

                if (crypto == null)
                {
                    throw new ArgumentNullException("Crypto not found");
                }

                var userPortfolioOfThisCrypto = user.Wallet.Portfolios.FirstOrDefault(p => p.CryptoCurrencyId == transactionDto.CryptoCurrencyId);

                if (userPortfolioOfThisCrypto == null)
                {
                    throw new ArgumentException("You don't own this crypto");
                }
                else if (userPortfolioOfThisCrypto.Quantity < transactionDto.Quantity)
                {
                    throw new ArgumentException("You don't have enough quantity of this crypto");
                }

                transaction.TransactionDate = DateTime.UtcNow;
                transaction.TransactionType = TransactionType.Sell;
                transaction.Price = crypto.CurrentPrice * transactionDto.Quantity;
                user.Wallet.Balance += transactionDto.Quantity * crypto.CurrentPrice;

                userPortfolioOfThisCrypto.Quantity -= transactionDto.Quantity;

                if (userPortfolioOfThisCrypto.Quantity == 0)
                {
                    _context.Portfolios.Remove(userPortfolioOfThisCrypto);
                }

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return _mapper.Map<TransactionDto>(transaction);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception("Transaction failed - Error selling crypto", ex);
            }
        }
    }
}
