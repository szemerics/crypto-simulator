using CryptoSimulator.DataContext.Dtos;
using CryptoSimulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoSimulator.Controllers
{
    [ApiController]
    [Route("api/users/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserData(int userId)
        {
            var user = await _userService.GetUserDataAsync(userId);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userDto)
        {
            var user = await _userService.RegisterUserAsync(userDto);
            return CreatedAtAction(nameof(GetUserData), new { userId = user.Id }, user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateDto userDto)
        {
            var user = await _userService.UpdateUserAsync(userId, userDto);
            return Ok(user);
        }

        [HttpDelete]
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
