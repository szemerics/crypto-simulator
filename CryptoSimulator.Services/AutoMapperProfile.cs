using AutoMapper;
using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CryptoCurrency, CryptoCurrencyDto>().ReverseMap();
            CreateMap<CryptoCurrencyCreateDto, CryptoCurrency>();

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserUpdateDto, User>();

            CreateMap<Wallet, WalletDto>().ReverseMap();

            CreateMap<Portfolio, PortfolioWalletDto>()
            .ForMember(dest => dest.CryptoCurrencySymbol,
                       opt => opt.MapFrom(src => src.CryptoCurrency.Symbol));

            CreateMap<PriceHistory, PriceHistoryDto>()
                .ForMember(dest => dest.CryptoCurrencySymbol, opt => opt.MapFrom(src => src.CryptoCurrency.Symbol))
                .ReverseMap();

            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.CryptoCurrencySymbol, opt => opt.MapFrom(src => src.CryptoCurrency.Symbol))
                .ReverseMap();
            CreateMap<TransactionCreateDto, Transaction>();
            CreateMap<Transaction, TransactionDetailedDto>()
                .ForMember(dest => dest.CryptoCurrencySymbol, opt => opt.MapFrom(src => src.CryptoCurrency.Symbol))
                .ForMember(dest => dest.PriceAtTransactionDate, opt => opt.MapFrom(src => src.Quantity == 0 ? 0 : src.Price / src.Quantity));

            CreateMap<LimitOrder, LimitOrderDto>()
                .ForMember(dest => dest.CryptoCurrencySymbol, opt => opt.MapFrom(src => src.CryptoCurrency.Symbol))
                .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.OrderType.ToString()))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()));
            CreateMap<LimitOrderCreateDto, LimitOrder>();
        }
    }
}
