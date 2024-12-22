using MindvizServer.Application.Utils;
using MindvizServer.Application.Interfaces;
using MindvizServer.Core.Interfaces;
using MindvizServer.Core.Models;

namespace MindvizServer.Application.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuth _authService;

        public UserService(IUserRepository userRepository, IAuth authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        public async Task<User?> RegisterUserAsync(User user)
        {
            if (user== null)
            {
                return null;
            }
            user.Password= PasswordHelper.GenerateHashedPassword(user.Password, user);
            return await _userRepository.CreateUserAsync(user);
        }
        public async Task<string?> LoginUserAsync(LoginModel loginModel)
        {
            User? user = await _userRepository.GetUserByEmailAsync(loginModel.Email);

            if (user == null)
            {
                Console.WriteLine("Login failed: User not found with email: " + loginModel.Email);
                return null;
            }

            if (!PasswordHelper.VerifyPassword(loginModel.Password, user.Password, user))
            {
                Console.WriteLine("Login failed: Password verification failed for email: " + loginModel.Email);
                return null;
            }

            Console.WriteLine("Login successful for user: " + user.Email);
            return _authService.GenerateToken(user);
        }





        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User> UpdateUserAsync(string id, User user)
        {
            return await _userRepository.UpdateUserAsync(id, user);
        }

        public async Task<User> DeleteUserAsync(string id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }
    }
}
