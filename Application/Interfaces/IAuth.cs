using MindvizServer.Core.Models;

namespace MindvizServer.Application.Interfaces
{
    public interface IAuth
    {
        public string GenerateToken(User user);
    }
}
