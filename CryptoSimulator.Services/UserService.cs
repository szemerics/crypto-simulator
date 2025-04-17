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
    public interface IUserService
    {
        Task<UserDto> GetUserDataAsync(int userId);
        Task<UserDto> RegisterUserAsync(UserRegisterDto userDto);
        // Task<string> LoginAsync(UserLoginDto userDto);

        Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto userDto);
        Task<bool> DeleteUserAsync(int userId);
    }

    public class UserService : IUserService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public UserService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserDataAsync (int userId)
        {
            var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentNullException("User not found");
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> RegisterUserAsync(UserRegisterDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            if (_context.Users.Select(u => u.Email == user.Email).Any())
            {
                throw new ArgumentException("User with this email already exists");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto userDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new ArgumentNullException("User not found");
            }

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            }

            _mapper.Map(userDto, user);
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new ArgumentNullException("User not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
