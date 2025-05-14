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
    public interface ILimitOrderService
    {
        Task<LimitOrderDto> CreateBuyLimitOrderAsync (int userId, LimitOrderCreateDto limitOrderDto);
        Task<LimitOrderDto> CreateSellLimitOrderAsync(int userId, LimitOrderCreateDto limitOrderDto);
        Task<List<LimitOrderDto>> GetLimitOrdersByUserIdAsync(int userId);
        Task<bool> DeleteLimitOrderAsync(int limitOrderId);

    }
    public class LimitOrderService : ILimitOrderService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;
        public LimitOrderService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<LimitOrderDto> CreateBuyLimitOrderAsync(int userId, LimitOrderCreateDto limitOrderDto)
        {
            var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(u => u.Wallet)
                    .ThenInclude(w => w.Portfolios)
                    .ThenInclude(p => p.CryptoCurrency)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var cryptoCurrency = await _context.CryptoCurrencies
                    .FirstOrDefaultAsync(c => c.Id == limitOrderDto.CryptoCurrencyId);


                // Validations
                if (user == null)
                {
                    throw new ArgumentNullException("User not found");
                }

                if (cryptoCurrency == null)
                {
                    throw new ArgumentNullException("Crypto currency not found");
                }

                if (user.Wallet.Balance < limitOrderDto.Quantity * limitOrderDto.LimitPrice)
                {
                    throw new ArgumentException("Insufficient balance");
                }

                if (limitOrderDto.LimitPrice <= 0)
                {
                    throw new ArgumentException("Limit price must be greater than zero");
                }

                var limitOrder = (new LimitOrder
                {
                    UserId = userId,
                    User = user,
                    CryptoCurrencyId = limitOrderDto.CryptoCurrencyId,
                    CryptoCurrency = cryptoCurrency,
                    Quantity = limitOrderDto.Quantity,
                    LimitPrice = limitOrderDto.LimitPrice,
                    CreatedAt = DateTime.UtcNow,
                    ExpireAt = limitOrderDto.ExpireAt,
                    OrderType = LimitOrderType.Buy,
                    OrderStatus = OrderStatus.Active
                });

                await _context.LimitOrders.AddAsync(limitOrder);
                await _context.SaveChangesAsync();

                await dbTransaction.CommitAsync();
                return _mapper.Map<LimitOrderDto>(limitOrder);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception("Transaction failed - Error creating limit-buy crypto", ex);
            }
        }

        public async Task<LimitOrderDto> CreateSellLimitOrderAsync(int userId, LimitOrderCreateDto limitOrderDto)
        {
            var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(u => u.Wallet)
                    .ThenInclude(w => w.Portfolios)
                    .ThenInclude(p => p.CryptoCurrency)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new ArgumentNullException("User not found");
                }

                var cryptoCurrency = await _context.CryptoCurrencies
                    .FirstOrDefaultAsync(c => c.Id == limitOrderDto.CryptoCurrencyId);

                if (cryptoCurrency == null)
                {
                    throw new ArgumentNullException("Crypto currency not found");
                }

                

                var doesTheUserOwn = user.Wallet.Portfolios
                    .Where(p => p.CryptoCurrencyId == limitOrderDto.CryptoCurrencyId)
                    .Any();

                if (!doesTheUserOwn)
                {
                    throw new ArgumentNullException("User does not own this crypto");
                } 
                else 
                {
                    var currentlyOwnedOfThisCrypto = user.Wallet.Portfolios
                        .FirstOrDefault(p => p.CryptoCurrencyId == limitOrderDto.CryptoCurrencyId).Quantity;

                    if (currentlyOwnedOfThisCrypto < limitOrderDto.Quantity)
                    {
                        throw new ArgumentException("User does not own enough crypto to sell");
                    }
                }


                if (limitOrderDto.LimitPrice <= 0)
                {
                    throw new ArgumentException("Limit price must be greater than zero");
                }

                var limitOrder = (new LimitOrder
                {
                    UserId = userId,
                    User = user,
                    CryptoCurrencyId = limitOrderDto.CryptoCurrencyId,
                    CryptoCurrency = cryptoCurrency,
                    Quantity = limitOrderDto.Quantity,
                    LimitPrice = limitOrderDto.LimitPrice,
                    CreatedAt = DateTime.UtcNow,
                    ExpireAt = limitOrderDto.ExpireAt,
                    OrderType = LimitOrderType.Sell,
                    OrderStatus = OrderStatus.Active
                });

                await _context.LimitOrders.AddAsync(limitOrder);
                await _context.SaveChangesAsync();

                await dbTransaction.CommitAsync();
                return _mapper.Map<LimitOrderDto>(limitOrder);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception("Transaction failed - Error creating limit-sell crypto", ex);
            }
        }

        public async Task<bool> DeleteLimitOrderAsync(int limitOrderId)
        {
            var limitOrder = await _context.LimitOrders
                .Where(o => o.Id == limitOrderId)
                .FirstOrDefaultAsync();

            if (limitOrder == null)
            {
                throw new ArgumentNullException("Limit order not found");
            }
            else
            {
                if (limitOrder.OrderStatus == OrderStatus.Completed)
                {
                    throw new ArgumentException("Limit order already executed");
                }
                else
                {
                    _context.LimitOrders.Remove(limitOrder);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
        }

        public async Task<List<LimitOrderDto>> GetLimitOrdersByUserIdAsync(int userId)
        {
            var limitOrders = await _context.LimitOrders
                .Where(o => o.UserId == userId)
                .Where(o => o.OrderStatus == OrderStatus.Active)
                .Include(o => o.CryptoCurrency)
                .ToListAsync();

            return _mapper.Map<List<LimitOrderDto>>(limitOrders);
        }
    }
}
