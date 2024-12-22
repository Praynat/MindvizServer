using MindvizServer.Application.Interfaces;
using MindvizServer.Core.Interfaces;
using MindvizServer.Core.Models;

namespace MindvizServer.Application.Services
{
    public class TaskService: ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskModel>> GetAllTasksAsync(string userId)
        {
            try
            {
                return await _taskRepository.GetTasksByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tasks for user {userId}: {ex.Message}");
                throw; 
            }
        }

        //ForAdmin
        public async Task<List<TaskModel>> GetTasksByUserIdAsync(string adminUserId, string targetUserId, bool isAdmin)
        {
            try
            {
                if (!isAdmin)
                {
                    throw new UnauthorizedAccessException("Only admins can access tasks of other users.");
                }

                return await _taskRepository.GetTasksByUserIdAsync(targetUserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tasks for user {targetUserId}: {ex.Message}");
                throw;
            }
        }

        public async Task<TaskModel?> CreateTaskAsync(TaskModel task, string userIdClaim)
        {
            try
            {
                task.UserId = userIdClaim;
                return await _taskRepository.CreateTaskAsync(task);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating task for user {userIdClaim}: {ex.Message}");
                throw;
            }
        }


        public async Task<TaskModel?> GetTaskByIdAsync(string id, string userId, bool isAdmin)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(id);
                if (task == null || (!isAdmin && task.UserId != userId))
                {
                    return null; // Task not found or unauthorized
                }
                return task;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching task by ID {id}: {ex.Message}");
                throw;
            }
        }


        public async Task<List<TaskModel>> GetTasksByNameAsync(string name, string userId, bool isAdmin)
        {
            try
            {
                var tasks = await _taskRepository.GetTasksByNameAsync(name);

            if (!isAdmin)
            {
                tasks = tasks.Where(t => t.UserId == userId).ToList();
            }

            return tasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tasks by name {name}: {ex.Message}");
                throw;
            }
        }



        public async Task<TaskModel?> UpdateTaskAsync(string id, TaskModel updatedTask, string userId, bool isAdmin)
        {
            try
            {
                var task = await GetTaskByIdAsync(id, userId, isAdmin);
                if (task == null)
                {
                    return null;
                }

                updatedTask.UserId = task.UserId; 
                return await _taskRepository.UpdateTaskAsync(id, updatedTask);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating task with ID {id}: {ex.Message}");
                throw;
            }
        }



        public async Task<TaskModel?> DeleteTaskAsync(string id, string userId, bool isAdmin)
        {
            try
            {
                var task = await GetTaskByIdAsync(id, userId, isAdmin);
                if (task == null)
                {
                    return null;
                }

                return await _taskRepository.DeleteTaskAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting task with ID {id}: {ex.Message}");
                throw;
            }
        }
    }
}
