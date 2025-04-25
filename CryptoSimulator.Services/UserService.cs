using AutoMapper;
using CryptoSimulator.DataContext.Context;
using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserDataAsync(int userId);
        Task<UserDto> RegisterUserAsync(UserRegisterDto userDto);
        Task<string> LoginAsync(UserLoginDto userDto);

        Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto userDto);
        Task<bool> DeleteUserAsync(int userId);
    }

    public class UserService : IUserService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(CryptoDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserDto> GetUserDataAsync (int userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentNullException("User not found");
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> RegisterUserAsync(UserRegisterDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                throw new ArgumentException("User with this email already exists");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            user.RoleId = 2; // "User" as default role -> can change to Admin in the database
            // Generating empty wallet with default Balance of 10000
            user.Wallet = new Wallet()
            {
                UserId = user.Id,
                User = user,
                Balance = 10000,
                Portfolios = new List<Portfolio>()
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto userDto)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

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

        public async Task<string> LoginAsync(UserLoginDto userDto)
        {
            var user = await _context.Users
                .Where(u => u.Email == userDto.Email)
                .Include(u => u.Role)
                .FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash)) {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return await GenerateTokenAsync(user);

        }

        // JWT Token Generation
        private async Task<string> GenerateTokenAsync(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var claims = await GetClaimsIdentity(user);
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims.Claims, expires: expires, signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.Now.ToString())
            };

            return new ClaimsIdentity(claims, "Token");
        }
    }
}
