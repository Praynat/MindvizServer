using MindvizServer.Core.Models;

namespace MindvizServer.Core.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAllUsersAsync();
        public Task<User> CreateUserAsync(User user);
        public Task<User> GetUserByIdAsync(string id);
        public Task<User> GetUserByNameAsync(string name);
        public Task<User> GetUserByEmailAsync(string email);
        public Task<User> UpdateUserAsync(string id, User user);
        public Task<User> DeleteUserAsync(string id);
    }
}
