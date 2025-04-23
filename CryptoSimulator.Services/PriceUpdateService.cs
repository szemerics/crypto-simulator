using CryptoSimulator.DataContext.Context;
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
    public class PriceUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Random _random = new Random();
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(60);

        public PriceUpdateService(IServiceProvider serviceProvider)
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

                    var cryptoCurrencies = await _context.CryptoCurrencies.ToListAsync(stoppingToken);

                    foreach (var crypto in cryptoCurrencies)
                    {
                        var changePercent = (_random.NextDouble() * 0.1 - 0.05);
                        var newPrice = crypto.CurrentPrice * (decimal)(1 + changePercent);

                        newPrice = Math.Round(newPrice, 2);
                        crypto.CurrentPrice = newPrice;

                        _context.PriceHistories.Add(new PriceHistory
                        {
                            CryptoCurrencyId = crypto.Id,
                            Price = newPrice,
                            Timestamp = DateTime.UtcNow
                        });
                    }

                    await _context.SaveChangesAsync(stoppingToken);
                }
                await Task.Delay(_updateInterval, stoppingToken);
            }
        }
    }
}
