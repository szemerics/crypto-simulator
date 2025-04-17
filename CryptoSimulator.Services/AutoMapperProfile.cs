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
        }
    }
}
