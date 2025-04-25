using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            var user = await _userService.RegisterUserAsync(userDto);
            return CreatedAtAction(nameof(GetUserData), new { userId = user.Id }, user);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            var token = await _userService.LoginAsync(userDto);
            return Ok(new { Token = token });
        }

        [HttpGet("myProfile")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var user = await _userService.GetUserDataAsync(userId);
            return Ok(user);
        }

        [HttpPut("updateMyProfile")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserUpdateDto userDto)
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var user = await _userService.UpdateUserAsync(userId, userDto);
            return Ok(user);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserData(int userId)
        {
            var user = await _userService.GetUserDataAsync(userId);
            return Ok(user);
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateDto userDto)
        {
            var user = await _userService.UpdateUserAsync(userId, userDto);
            return Ok(user);
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
