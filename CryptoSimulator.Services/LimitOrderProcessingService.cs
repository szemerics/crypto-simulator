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
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10);

        public LimitOrderProcessingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var limitOrderService = scope.ServiceProvider.GetRequiredService<ILimitOrderService>();
                        await limitOrderService.ProcessLimitOrdersAsync();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error at LimitOrder at " + DateTime.Now);
                    continue;
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
