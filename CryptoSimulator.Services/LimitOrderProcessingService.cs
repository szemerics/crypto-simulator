using CryptoSimulator.DataContext.Context;
using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.Services
{
    public class LimitOrderProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(10);

        public LimitOrderProcessingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<CryptoDbContext>();
                    var _transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

                    var limitOrders = await _context.LimitOrders.ToListAsync(stoppingToken);
                    
                    foreach (var order in limitOrders)
                    {   
                        if (order.OrderStatus != OrderStatus.Active)
                        {
                            continue;
                        }

                        if (order.ExpireAt < DateTime.UtcNow)
                        {
                            order.OrderStatus = OrderStatus.Expired;
                        }
                        else
                        {
                            var cryptoCurrency = await _context.CryptoCurrencies
                                .Where(c => c.Id == order.CryptoCurrencyId)
                                .FirstOrDefaultAsync(stoppingToken);

                            if (cryptoCurrency == null)
                            {
                                throw new Exception("CryptoCurrency not found");
                            }

                            var user = await _context.Users
                                .Where(u => u.Id == order.UserId)
                                .Include(u => u.Wallet)
                                .ThenInclude(w => w.Portfolios)
                                .ThenInclude(p => p.CryptoCurrency)
                                .FirstOrDefaultAsync(stoppingToken);

                            if (user == null)
                            {
                                throw new Exception("User not found");
                            }

                            if (order.OrderType == LimitOrderType.Buy)
                            {
                                var requiredWalletBalance = order.Quantity * order.LimitPrice;

                                if (cryptoCurrency.CurrentPrice <= order.LimitPrice && user.Wallet.Balance >= requiredWalletBalance)
                                {
                                    user.Wallet.Balance -= requiredWalletBalance;

                                    var userPortfolio = user.Wallet.Portfolios
                                        .Where(p => p.CryptoCurrencyId == order.CryptoCurrencyId)
                                        .FirstOrDefault();

                                    if (userPortfolio != null)
                                    {
                                        userPortfolio.Quantity += order.Quantity;

                                        order.OrderStatus = OrderStatus.Completed;
                                    }
                                    else
                                    {
                                        userPortfolio = new Portfolio
                                        {
                                            CryptoCurrencyId = order.CryptoCurrencyId,
                                            Quantity = order.Quantity,
                                            PurchasePrice = cryptoCurrency.CurrentPrice,
                                            WalletId = user.Wallet.Id
                                        };
                                        await _context.Portfolios.AddAsync(userPortfolio, stoppingToken);

                                        var transactionDto = new TransactionCreateDto
                                        {
                                            CryptoCurrencyId = order.CryptoCurrencyId,
                                            Quantity = order.Quantity,
                                        };

                                        await _transactionService.BuyTransactionAsync(user.Id, transactionDto);

                                        order.OrderStatus = OrderStatus.Completed;
                                    }
                                }
                            }
                            else if (order.OrderType == LimitOrderType.Sell)
                            {
                                if (cryptoCurrency.CurrentPrice >= order.LimitPrice)
                                {
                                    var userPortfolio = user.Wallet.Portfolios
                                        .Where(p => p.CryptoCurrencyId == order.CryptoCurrencyId)
                                        .FirstOrDefault();

                                    if (userPortfolio != null && userPortfolio.Quantity >= order.Quantity)
                                    {
                                        userPortfolio.Quantity -= order.Quantity;

                                        user.Wallet.Balance += order.Quantity * cryptoCurrency.CurrentPrice;

                                        if (userPortfolio.Quantity == 0)
                                        {
                                            _context.Portfolios.Remove(userPortfolio);
                                        }

                                        var transactionDto = new TransactionCreateDto
                                        {
                                            CryptoCurrencyId = order.CryptoCurrencyId,
                                            Quantity = order.Quantity,
                                        };

                                        await _transactionService.BuyTransactionAsync(user.Id, transactionDto);

                                        order.OrderStatus = OrderStatus.Completed;
                                    }
                                    else
                                    {
                                        throw new Exception("Insufficient quantity to sell");
                                    }
                                }
                            }

                            
                        }
                    }

                    await _context.SaveChangesAsync(stoppingToken);
                }
                await Task.Delay(_updateInterval, stoppingToken);
            }
        }
    }
}
