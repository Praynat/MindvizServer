using MindvizServer.Core.Interfaces;
using MindvizServer.Core.Models;
using MindvizServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MindvizServer.Core.Models.SubModels;


namespace MindvizServer.Infrastructure.Services
{
    public class TaskRepository: ITaskRepository
    {
        private readonly MindvizDbContext _context;

        public TaskRepository(MindvizDbContext context)
        {
            _context = context;
        }
        public async Task<List<TaskModel>> GetTasksByUserIdAsync(string userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }



        public async Task<TaskModel?> CreateTaskAsync(TaskModel task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskModel?> GetTaskByIdAsync(string id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<List<TaskModel>> GetTasksByNameAsync(string name)
        {
            
           return await _context.Tasks
           .Where(t => t.Name == name)
           .ToListAsync();
           
        }


        public async Task<TaskModel?> UpdateTaskAsync(string id, TaskModel updatedTask)
        {
           
                var taskToUpdate = await _context.Tasks.FindAsync(id);
                if (taskToUpdate == null)
                {
                    return null; // Task not found
                }

                // Update properties
                taskToUpdate.Name = updatedTask.Name;
                taskToUpdate.Description = updatedTask.Description;
                taskToUpdate.ParentIds = updatedTask.ParentIds;
                taskToUpdate.ChildrenIds = updatedTask.ChildrenIds;
                taskToUpdate.Type = updatedTask.Type;
                taskToUpdate.Progress = updatedTask.Progress;
                taskToUpdate.Weight = updatedTask.Weight;
                taskToUpdate.IsDeadline = updatedTask.IsDeadline;
                taskToUpdate.Deadline = updatedTask.Deadline;
                taskToUpdate.UpdatedAt = DateTime.Now; // Update timestamp
                taskToUpdate.IsFrequency = updatedTask.IsFrequency;
                taskToUpdate.Frequency = updatedTask.Frequency;
                taskToUpdate.StartDate = updatedTask.StartDate;
                taskToUpdate.EndDate = updatedTask.EndDate;
                taskToUpdate.NextOccurrences = updatedTask.NextOccurrences;
                taskToUpdate.WeekDays = updatedTask.WeekDays;
                taskToUpdate.DayOfMonth = updatedTask.DayOfMonth;
                taskToUpdate.MonthOfYear = updatedTask.MonthOfYear;
                taskToUpdate.FrequencyInterval = updatedTask.FrequencyInterval;
                taskToUpdate.AssignedTo = updatedTask.AssignedTo;
                taskToUpdate.Links = updatedTask.Links;
                taskToUpdate.Tags = updatedTask.Tags;
                taskToUpdate.UserTasks = updatedTask.UserTasks;
                taskToUpdate.UserId = updatedTask.UserId;

                // Save changes to the database
                await _context.SaveChangesAsync();
                return taskToUpdate;
            
        }


        public async Task<TaskModel?> DeleteTaskAsync(string id)
        {
            
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                {
                    return null; // User not found
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return task;
            
        }
        
    }
}

