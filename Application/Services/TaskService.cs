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
                task.Id = task.Id ?? Guid.NewGuid().ToString(); // Generate ID if not provided
                task.ParentIds ??= new List<string>();
                task.ChildrenIds ??= new List<string>();
                task.Tags ??= new List<string>();
                task.Links ??= new List<string>();
                task.NextOccurrences ??= new List<DateTime>();
                task.UserTasks ??= new List<UserTask>();
                task.CreatedAt = DateTime.Now;
                if (task.Weight <= 0)
                {
                    task.Weight = 1.0;
                }
                // Create the task
                var createdTask = await _taskRepository.CreateTaskAsync(task);

                // If this task has parent tasks, update their progress
                if (createdTask != null && createdTask.ParentIds.Count > 0)
                {
                    foreach (var parentId in createdTask.ParentIds)
                    {
                        var parentTask = await _taskRepository.GetTaskByIdAsync(parentId);
                        if (parentTask != null && !parentTask.IsChecked)
                        {
                            // Add this task to the parent's children if not already there
                            if (!parentTask.ChildrenIds.Contains(createdTask.Id))
                            {
                                parentTask.ChildrenIds.Add(createdTask.Id);
                            }

                            // Calculate the parent's progress based on all its children
                            double totalWeightedProgress = 0;
                            double totalWeight = 0;

                            foreach (var childId in parentTask.ChildrenIds)
                            {
                                var childTask = await _taskRepository.GetTaskByIdAsync(childId);
                                if (childTask != null)
                                {
                                    totalWeightedProgress += childTask.Progress * childTask.Weight;
                                    totalWeight += childTask.Weight;
                                }
                            }

                            if (totalWeight > 0)
                            {
                                parentTask.Progress = (int)(totalWeightedProgress / totalWeight);
                                await _taskRepository.UpdateTaskAsync(parentId, parentTask);
                            }
                        }
                    }
                }

                return createdTask;
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
                var existingTask = await GetTaskByIdAsync(id, userId, isAdmin);
                if (existingTask == null)
                {
                    return null;
                }

                updatedTask.UserId = existingTask.UserId;

                // Check if the task is being unchecked (was previously checked but now is not)
                bool isBeingUnchecked = existingTask.IsChecked && !updatedTask.IsChecked;

                // If the task is checked, set progress to 100%
                if (updatedTask.IsChecked)
                {
                    updatedTask.Progress = 100;
                }
                // If the task is being explicitly unchecked, reset progress to 0 regardless of children
                else if (isBeingUnchecked)
                {
                    updatedTask.Progress = 0;
                    Console.WriteLine($"Task '{updatedTask.Name}' was unchecked, resetting progress to 0");
                }
                // If it has children and is not checked, calculate progress as weighted average of children
                else if (updatedTask.ChildrenIds.Count > 0)
                {
                    double totalWeightedProgress = 0;
                    double totalWeight = 0;

                    foreach (var childId in updatedTask.ChildrenIds)
                    {
                        var childTask = await _taskRepository.GetTaskByIdAsync(childId);
                        if (childTask != null)
                        {
                            totalWeightedProgress += childTask.Progress * childTask.Weight;
                            totalWeight += childTask.Weight;
                        }
                    }

                    // Only update the progress if we found valid children
                    if (totalWeight > 0)
                    {
                        updatedTask.Progress = (int)(totalWeightedProgress / totalWeight);
                    }
                    else
                    {
                        // If no valid children found, set to 0
                        updatedTask.Progress = 0;
                    }
                }
                // If the task has no children and is not checked, progress is 0
                else
                {
                    updatedTask.Progress = 0;
                }

                // Update the task
                var result = await _taskRepository.UpdateTaskAsync(id, updatedTask);

                // Now update progress of any parent tasks that include this task
                if (result != null && updatedTask.ParentIds.Count > 0)
                {
                    foreach (var parentId in updatedTask.ParentIds)
                    {
                        var parentTask = await _taskRepository.GetTaskByIdAsync(parentId);
                        if (parentTask != null && !parentTask.IsChecked)
                        {
                            // Calculate the parent's progress based on all its children
                            double totalWeightedProgress = 0;
                            double totalWeight = 0;

                            foreach (var childId in parentTask.ChildrenIds)
                            {
                                var childTask = await _taskRepository.GetTaskByIdAsync(childId);
                                if (childTask != null)
                                {
                                    totalWeightedProgress += childTask.Progress * childTask.Weight;
                                    totalWeight += childTask.Weight;
                                }
                            }

                            if (totalWeight > 0)
                            {
                                parentTask.Progress = (int)(totalWeightedProgress / totalWeight);
                                await _taskRepository.UpdateTaskAsync(parentId, parentTask);
                            }
                        }
                    }
                }

                return result;
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
                if (task.IsRoot)
                {
                    Console.WriteLine($"Cannot delete task {id} because it is marked as a root task.");
                    throw new InvalidOperationException("Root tasks cannot be deleted.");
                }
                // Store parent IDs before deleting the task
                var parentIds = new List<string>(task.ParentIds);

                // Delete the task
                var deletedTask = await _taskRepository.DeleteTaskAsync(id);

                if (deletedTask != null && parentIds.Count > 0)
                {
                    // Update progress of each parent task after removing this child
                    foreach (var parentId in parentIds)
                    {
                        var parentTask = await _taskRepository.GetTaskByIdAsync(parentId);
                        if (parentTask != null && !parentTask.IsChecked)
                        {
                            // Remove this task from parent's children if needed
                            if (parentTask.ChildrenIds.Contains(id))
                            {
                                parentTask.ChildrenIds.Remove(id);
                            }

                            // Recalculate parent's progress based on remaining children
                            double totalWeightedProgress = 0;
                            double totalWeight = 0;

                            foreach (var childId in parentTask.ChildrenIds)
                            {
                                var childTask = await _taskRepository.GetTaskByIdAsync(childId);
                                if (childTask != null)
                                {
                                    totalWeightedProgress += childTask.Progress * childTask.Weight;
                                    totalWeight += childTask.Weight;
                                }
                            }

                            // Update parent's progress
                            if (totalWeight > 0)
                            {
                                parentTask.Progress = (int)(totalWeightedProgress / totalWeight);
                            }
                            else
                            {
                                // If no children left, progress is 0 (unless manually checked)
                                parentTask.Progress = parentTask.IsChecked ? 100 : 0;
                            }

                            await _taskRepository.UpdateTaskAsync(parentId, parentTask);
                        }
                    }
                }

                return deletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting task with ID {id}: {ex.Message}");
                throw;
            }
        }
    }
}
