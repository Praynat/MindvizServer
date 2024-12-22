using MindvizServer.Core.Models;

namespace MindvizServer.Application.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskModel>> GetAllTasksAsync(string userId);
        Task<List<TaskModel>> GetTasksByUserIdAsync(string adminUserId, string targetUserId, bool isAdmin);
        Task<TaskModel?> CreateTaskAsync(TaskModel task, string userIdClaim);
        Task<TaskModel?> GetTaskByIdAsync(string id, string userId, bool isAdmin);
        Task<List<TaskModel>> GetTasksByNameAsync(string name, string userId, bool isAdmin);
        Task<TaskModel?> UpdateTaskAsync(string id, TaskModel updatedTask, string userId, bool isAdmin);
        Task<TaskModel?> DeleteTaskAsync(string id, string userId, bool isAdmin);
    }
}
