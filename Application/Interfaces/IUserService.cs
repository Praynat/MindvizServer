using MindvizServer.Core.Models;

namespace MindvizServer.Application.Interfaces
{
    public interface IUserService
    {
        public Task<User> RegisterUserAsync(User user);
        public Task<string?> LoginUserAsync(LoginModel loginModel);
        public Task<List<User>> GetAllUsersAsync();
        public Task<User> GetUserByIdAsync(string id);
        public Task<User> UpdateUserAsync(string id, User user);
        public Task<User> DeleteUserAsync(string id);

    }
}
