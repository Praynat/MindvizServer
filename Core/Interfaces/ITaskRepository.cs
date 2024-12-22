using MindvizServer.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MindvizServer.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskModel>> GetTasksByUserIdAsync(string userId);
        Task<TaskModel?> CreateTaskAsync(TaskModel task);
        Task<TaskModel?> GetTaskByIdAsync(string id);
        Task<List<TaskModel>> GetTasksByNameAsync(string name);
        Task<TaskModel?> UpdateTaskAsync(string id, TaskModel task);
        Task<TaskModel?> DeleteTaskAsync(string id);
    }
}
