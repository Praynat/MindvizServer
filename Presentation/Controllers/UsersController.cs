
using MindvizServer.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MindvizServer.Application.Interfaces;

namespace MindvizServer.Presentation.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
           User? result = await _userService.RegisterUserAsync(user);
            if (result==null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            Console.WriteLine($"Login endpoint hit with email: {loginModel.Email}");

            string? result = await _userService.LoginUserAsync(loginModel);
            if (result == null)
            {
                Console.WriteLine("Login failed: Unauthorized");
                return Unauthorized();
            }

            Console.WriteLine("Login successful: Token generated");
            return Ok(new { Token = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            List<User> result = await _userService.GetAllUsersAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById( string id )
        {
            User? result = await _userService.GetUserByIdAsync(id);
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            User result = await _userService.UpdateUserAsync(id, user);
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            User result = await _userService.DeleteUserAsync(id);
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }
    }
}
