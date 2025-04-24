using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MindvizServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MindvizServer.Infrastructure.Data
{
    public class TaskDataMigration : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskDataMigration(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Create a scope to get scoped services
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MindvizDbContext>();

                // Get all tasks
                var tasks = await context.Tasks.ToListAsync(cancellationToken);

                Console.WriteLine($"Starting task progress recalculation for {tasks.Count} tasks...");

                // First pass: Set progress based on IsChecked for tasks without children
                foreach (var task in tasks.Where(t => t.ChildrenIds == null || !t.ChildrenIds.Any()))
                {
                    // If task is checked, set progress to 100
                    if (task.IsChecked)
                    {
                        task.Progress = 100;
                    }
                    else
                    {
                        // For tasks with no children and not checked, reset progress to 0
                        task.Progress = 0;
                    }
                }

                // Save first batch of changes
                await context.SaveChangesAsync(cancellationToken);

                // Second pass: Calculate progress for parent tasks
                var parentTasks = tasks
                    .Where(t => t.ChildrenIds != null && t.ChildrenIds.Any())
                    .OrderBy(t => t.ChildrenIds.Count);

                foreach (var parentTask in parentTasks)
                {
                    // If task is checked, set progress to 100 regardless of children
                    if (parentTask.IsChecked)
                    {
                        parentTask.Progress = 100;
                        continue;
                    }

                    // Calculate progress based on children
                    double totalWeightedProgress = 0;
                    double totalWeight = 0;

                    foreach (var childId in parentTask.ChildrenIds)
                    {
                        var childTask = tasks.FirstOrDefault(t => t.Id == childId);
                        if (childTask != null)
                        {
                            totalWeightedProgress += childTask.Progress * childTask.Weight;
                            totalWeight += childTask.Weight;
                        }
                    }

                    // Update progress
                    if (totalWeight > 0)
                    {
                        parentTask.Progress = (int)(totalWeightedProgress / totalWeight);
                    }
                    else
                    {
                        // If no valid children found, set to 0
                        parentTask.Progress = 0;
                    }
                }

                // Save all changes
                await context.SaveChangesAsync(cancellationToken);

                Console.WriteLine("Task progress recalculation completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during task data migration: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
