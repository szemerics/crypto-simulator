using AutoMapper;
using BCrypt.Net;
using CryptoSimulator.DataContext.Context;
using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CryptoSimulator.Services
{
    public interface ILimitOrderService
    {
        Task<LimitOrderDto> CreateLimitBuyOrderAsync (int userId, LimitOrderCreateDto limitOrderCreateDto);
        Task<LimitOrderDto> CreateLimitSellOrderAsync(int userId, LimitOrderCreateDto limitOrderCreateDto);
        Task<List<LimitOrderDto>> GetLimitOrdersByUserIdAsync(int userId);
        Task<bool> CancelLimitOrderAsync(int orderId, int userId);
        Task ProcessLimitOrdersAsync();
    }

    public class LimitOrderService : ILimitOrderService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;

        public LimitOrderService(CryptoDbContext context, IMapper mapper, ITransactionService transactionService)
        {
            _context = context;
            _mapper = mapper;
            _transactionService = transactionService;
        }

        public async Task<bool> CancelLimitOrderAsync(int orderId, int userId)
        {
            try
            {
                var limitOrder = await _context.LimitOrders
                    .FirstOrDefaultAsync(lo => lo.Id == orderId && lo.UserId == userId);

                if (limitOrder == null)
                {
                    throw new ArgumentException("Limit order not found");
                }

                if (limitOrder.Status != LimitOrderStatus.Active)
                {
                    throw new ArgumentException("Cannot cancel an order that is not active");
                }

                var user = await _context.Users
                    .Include(u => u.Wallet)
                    .ThenInclude(w => w.Portfolios)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }

                // Return funds or crypto based on order type
                if (limitOrder.OrderType == LimitOrderType.Buy)
                {
                    // Return the reserved funds
                    decimal refundAmount = limitOrder.Quantity * limitOrder.LimitPrice;
                    user.Wallet.Balance += refundAmount;
                }
                else if (limitOrder.OrderType == LimitOrderType.Sell)
                {
                    // Return the reserved crypto
                    var portfolio = user.Wallet.Portfolios
                        .FirstOrDefault(p => p.CryptoCurrencyId == limitOrder.CryptoCurrencyId);

                    if (portfolio == null)
                    {
                        // Create new portfolio if it doesn't exist
                        portfolio = new Portfolio
                        {
                            WalletId = user.Wallet.Id,
                            CryptoCurrencyId = limitOrder.CryptoCurrencyId,
                            Quantity = limitOrder.Quantity,
                            PurchasePrice = await _context.CryptoCurrencies
                                .Where(c => c.Id == limitOrder.CryptoCurrencyId)
                                .Select(c => c.CurrentPrice)
                                .FirstOrDefaultAsync()
                        };
                        user.Wallet.Portfolios.Add(portfolio);
                    }
                    else
                    {
                        // Add quantity to existing portfolio
                        portfolio.Quantity += limitOrder.Quantity;
                    }
                }

                // Update order status
                limitOrder.Status = LimitOrderStatus.Cancelled;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to cancel limit order", ex);
            }
        }

        public async Task<LimitOrderDto> CreateLimitBuyOrderAsync(int userId, LimitOrderCreateDto limitOrderCreateDto)
        {
            var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(u => u.Wallet)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var cryptoCurrency = await _context.CryptoCurrencies
                    .FirstOrDefaultAsync(c => c.Id == limitOrderCreateDto.CryptoCurrencyId);

                // Validations

                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }


                if (cryptoCurrency == null)
                {
                    throw new ArgumentException("Cryptocurrency not found");
                }

                if (limitOrderCreateDto.LimitPrice <= 0)
                {
                    throw new ArgumentException("Limit price must be greater than zero");
                }

                if (limitOrderCreateDto.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero");
                }

                decimal requiredBalance = limitOrderCreateDto.Quantity * limitOrderCreateDto.LimitPrice;
                if (user.Wallet.Balance < requiredBalance)
                {
                    throw new ArgumentException("Insufficient balance");
                }

                var limitOrder = new LimitOrder
                {
                    UserId = userId,
                    CryptoCurrencyId = limitOrderCreateDto.CryptoCurrencyId,
                    OrderType = LimitOrderType.Buy,
                    Status = LimitOrderStatus.Active,
                    Quantity = limitOrderCreateDto.Quantity,
                    LimitPrice = limitOrderCreateDto.LimitPrice,
                    CreatedAt = DateTime.UtcNow,
                    ExpirationTime = limitOrderCreateDto.ExpirationTime
                };

                user.Wallet.Balance -= requiredBalance;

                await _context.LimitOrders.AddAsync(limitOrder);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return _mapper.Map<LimitOrderDto>(limitOrder);

            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception("Transaction failed - Error creating limit buy order", ex);
            }
        }

        public async Task<LimitOrderDto> CreateLimitSellOrderAsync(int userId, LimitOrderCreateDto limitOrderCreateDto)
        {
            var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var user = await _context.Users
                    .Include(u => u.Wallet)
                    .ThenInclude(w => w.Portfolios)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var crypto = await _context.CryptoCurrencies
                    .FirstOrDefaultAsync(c => c.Id == limitOrderCreateDto.CryptoCurrencyId);

                var portfolio = user.Wallet.Portfolios
                    .FirstOrDefault(p => p.CryptoCurrencyId == limitOrderCreateDto.CryptoCurrencyId);

                // Validations

                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }

                if (crypto == null)
                {
                    throw new ArgumentException("Cryptocurrency not found");
                }

                if (limitOrderCreateDto.LimitPrice <= 0)
                {
                    throw new ArgumentException("Limit price must be greater than zero");
                }

                if (limitOrderCreateDto.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero");
                }

                if (portfolio == null || portfolio.Quantity < limitOrderCreateDto.Quantity)
                {
                    throw new ArgumentException("Insufficient cryptocurrency balance");
                }

                var limitOrder = new LimitOrder
                {
                    UserId = userId,
                    CryptoCurrencyId = limitOrderCreateDto.CryptoCurrencyId,
                    OrderType = LimitOrderType.Sell,
                    Status = LimitOrderStatus.Active,
                    Quantity = limitOrderCreateDto.Quantity,
                    LimitPrice = limitOrderCreateDto.LimitPrice,
                    CreatedAt = DateTime.UtcNow,
                    ExpirationTime = limitOrderCreateDto.ExpirationTime
                };

                // Reserve the crypto by removing it from portfolio
                portfolio.Quantity -= limitOrderCreateDto.Quantity;
                
                // If portfolio is empty after removing crypto, remove it
                if (portfolio.Quantity == 0)
                {
                    _context.Portfolios.Remove(portfolio);
                }

                await _context.LimitOrders.AddAsync(limitOrder);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return _mapper.Map<LimitOrderDto>(limitOrder);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception("Transaction failed - Error creating limit sell order", ex);
            }
        }

        public async Task<List<LimitOrderDto>> GetLimitOrdersByUserIdAsync(int userId)
        {
            var limitOrders = await _context.LimitOrders
                .Include(lo => lo.CryptoCurrency)
                .Where(lo => lo.UserId == userId && lo.Status == LimitOrderStatus.Active)
                .ToListAsync();

            return _mapper.Map<List<LimitOrderDto>>(limitOrders);
        }

        public async Task ProcessLimitOrdersAsync()
        {
            try
            {
                // Process expired orders first
                await ProcessExpiredOrdersAsync();

                // Get active orders
                var activeOrders = await _context.LimitOrders
                   .Include(lo => lo.CryptoCurrency)
                   .Where(lo => lo.Status == LimitOrderStatus.Active)
                   .ToListAsync();

                // Process each order individually
                foreach (var order in activeOrders)
                {
                    try
                    {
                        var crypto = await _context.CryptoCurrencies
                            .FirstOrDefaultAsync(c => c.Id == order.CryptoCurrencyId);

                        if (crypto == null)
                        {
                            continue;
                        }

                        bool shouldExecute = false;

                        if (order.OrderType == LimitOrderType.Buy && crypto.CurrentPrice <= order.LimitPrice)
                        {
                            shouldExecute = true;
                        }
                        else if (order.OrderType == LimitOrderType.Sell && crypto.CurrentPrice >= order.LimitPrice)
                        {
                            shouldExecute = true;
                        }

                        if (shouldExecute)
                        {
                            var transactionDto = new TransactionCreateDto
                            {
                                CryptoCurrencyId = order.CryptoCurrencyId,
                                Quantity = order.Quantity
                            };

                            // First step: Return reserved funds/crypto (with transaction scope)
                            using (var returnScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                            {
                                try
                                {
                                    if (order.OrderType == LimitOrderType.Buy)
                                    {
                                        // Return the reserved funds
                                        var user = await _context.Users
                                            .Include(u => u.Wallet)
                                            .FirstOrDefaultAsync(u => u.Id == order.UserId);

                                        if (user == null)
                                        {
                                            continue;
                                        }
                                        
                                        // Return the reserved funds
                                        decimal reservedAmount = order.Quantity * order.LimitPrice;
                                        user.Wallet.Balance += reservedAmount;
                                    }
                                    else if (order.OrderType == LimitOrderType.Sell)
                                    {
                                        // Return the reserved crypto
                                        var user = await _context.Users
                                            .Include(u => u.Wallet)
                                            .ThenInclude(w => w.Portfolios)
                                            .FirstOrDefaultAsync(u => u.Id == order.UserId);

                                        if (user == null)
                                        {
                                            continue;
                                        }
                                        
                                        var portfolio = user.Wallet.Portfolios
                                            .FirstOrDefault(p => p.CryptoCurrencyId == order.CryptoCurrencyId);
                                            
                                        if (portfolio == null)
                                        {
                                            portfolio = new Portfolio
                                            {
                                                WalletId = user.Wallet.Id,
                                                CryptoCurrencyId = order.CryptoCurrencyId,
                                                Quantity = order.Quantity,
                                                PurchasePrice = crypto.CurrentPrice
                                            };
                                            user.Wallet.Portfolios.Add(portfolio);
                                        }
                                        else
                                        {
                                            portfolio.Quantity += order.Quantity;
                                        }
                                    }
                                    
                                    // Save return of funds/crypto
                                    await _context.SaveChangesAsync();
                                    returnScope.Complete();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error returning assets for order {0}: {1}", order.Id, ex.Message);
                                    continue; // Skip to the next order
                                }
                            }

                            // Second step: Execute transaction (without transaction scope, TransactionService has its own)
                            try
                            {
                                if (order.OrderType == LimitOrderType.Buy)
                                {
                                    await _transactionService.BuyTransactionAsync(order.UserId, transactionDto);
                                }
                                else if (order.OrderType == LimitOrderType.Sell)
                                {
                                    await _transactionService.SellTransactionAsync(order.UserId, transactionDto);
                                }
                                
                                // Third step: Update order status (with transaction scope)
                                using (var updateScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                                {
                                    order.Status = LimitOrderStatus.Completed;
                                    order.CompletedAt = DateTime.UtcNow;
                                    await _context.SaveChangesAsync();
                                    updateScope.Complete();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error executing transaction for order {0}: {1}", order.Id, ex.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing order {0}: {1}", order.Id, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ProcessLimitOrdersAsync: " + ex.Message);
            }
        }

        private async Task ProcessExpiredOrdersAsync()
        {
            var now = DateTime.UtcNow;
            var expiredOrders = await _context.LimitOrders
                .Where(lo => lo.Status == LimitOrderStatus.Active &&
                            lo.ExpirationTime != null &&
                            lo.ExpirationTime < now)
                .ToListAsync();

            foreach (var order in expiredOrders)
            {
                try
                {
                    // First step: Cancel the order (return funds/crypto)
                    try
                    {
                        using (var cancelScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            // Get the order with its details
                            var limitOrder = await _context.LimitOrders
                                .FirstOrDefaultAsync(lo => lo.Id == order.Id);

                            if (limitOrder == null || limitOrder.Status != LimitOrderStatus.Active)
                            {
                                continue;
                            }

                            var user = await _context.Users
                                .Include(u => u.Wallet)
                                .ThenInclude(w => w.Portfolios)
                                .FirstOrDefaultAsync(u => u.Id == order.UserId);

                            if (user == null)
                            {
                                continue;
                            }

                            // Return funds or crypto based on order type
                            if (limitOrder.OrderType == LimitOrderType.Buy)
                            {
                                // Return the reserved funds
                                decimal refundAmount = limitOrder.Quantity * limitOrder.LimitPrice;
                                user.Wallet.Balance += refundAmount;
                            }
                            else if (limitOrder.OrderType == LimitOrderType.Sell)
                            {
                                // Return the reserved crypto
                                var portfolio = user.Wallet.Portfolios
                                    .FirstOrDefault(p => p.CryptoCurrencyId == limitOrder.CryptoCurrencyId);

                                if (portfolio == null)
                                {
                                    // Create new portfolio if it doesn't exist
                                    portfolio = new Portfolio
                                    {
                                        WalletId = user.Wallet.Id,
                                        CryptoCurrencyId = limitOrder.CryptoCurrencyId,
                                        Quantity = limitOrder.Quantity,
                                        PurchasePrice = await _context.CryptoCurrencies
                                            .Where(c => c.Id == limitOrder.CryptoCurrencyId)
                                            .Select(c => c.CurrentPrice)
                                            .FirstOrDefaultAsync()
                                    };
                                    user.Wallet.Portfolios.Add(portfolio);
                                }
                                else
                                {
                                    // Add quantity to existing portfolio
                                    portfolio.Quantity += limitOrder.Quantity;
                                }
                            }

                            await _context.SaveChangesAsync();
                            cancelScope.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error cancelling expired order {0}: {1}", order.Id, ex.Message);
                        continue; // Skip to next order
                    }

                    // Second step: Update order status
                    try
                    {
                        using (var updateScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            // Get the order again to avoid tracking issues
                            var orderToUpdate = await _context.LimitOrders
                                .FirstOrDefaultAsync(lo => lo.Id == order.Id);
                                
                            if (orderToUpdate != null && orderToUpdate.Status == LimitOrderStatus.Active)
                            {
                                orderToUpdate.Status = LimitOrderStatus.Expired;
                                await _context.SaveChangesAsync();
                            }
                            
                            updateScope.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error updating expired order status {0}: {1}", order.Id, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error processing expired order {0}: {1}", order.Id, ex.Message);
                }
            }
        }
    }
}
