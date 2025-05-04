using Microsoft.EntityFrameworkCore;
using MindvizServer.Core.Interfaces;
using MindvizServer.Core.Models;
using MindvizServer.Infrastructure.Data;
using System.Text.Json;

namespace MindvizServer.Infrastructure.Services
{
    public class GroupRepository : IGroupRepository
    {
        private readonly MindvizDbContext _context;

        public GroupRepository(MindvizDbContext context)
        {
            _context = context;
        }

        // Basic CRUD operations
        public async Task<List<Group>> GetAllGroupsAsync()
        {
            return await _context.Groups
                .Include(g => g.Members)
                .Include(g => g.Tasks)
                .ToListAsync();
        }

        public async Task<Group> GetGroupByIdAsync(string id)
        {
            return await _context.Groups
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .Include(g => g.Tasks)
                    .ThenInclude(t => t.Task)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<Group> UpdateGroupAsync(string id, Group updatedGroup)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return null;

            // Update basic properties
            group.Name = updatedGroup.Name;
            group.Description = updatedGroup.Description;
            group.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<Group> DeleteGroupAsync(string id)
        {
            var group = await _context.Groups
                .Include(g => g.Members)
                .Include(g => g.Tasks)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
                return null;
            foreach (var groupTask in group.Tasks)
            {
                if (groupTask.Task != null && groupTask.Task.IsRoot)
                {
                    Console.WriteLine($"Cannot delete group {id} because it contains root task {groupTask.TaskId}.");
                    throw new InvalidOperationException("Cannot delete a group that contains root tasks.");
                }
            }
            _context.GroupMembers.RemoveRange(group.Members);
            _context.GroupTasks.RemoveRange(group.Tasks);
            _context.Groups.Remove(group);

            await _context.SaveChangesAsync();
            return group;
        }

        // Additional query methods
        public async Task<List<Group>> GetGroupsByUserIdAsync(string userId)
        {
            return await _context.Groups
                .Include(g => g.Members)
                .Include(g => g.Tasks)
                .Where(g => g.Members.Any(m => m.UserId == userId))
                .ToListAsync();
        }

        public async Task<List<Group>> GetGroupsByCreatorIdAsync(string creatorId)
        {
            return await _context.Groups
                .Include(g => g.Members)
                .Include(g => g.Tasks)
                .Where(g => g.CreatorId == creatorId)
                .ToListAsync();
        }

        // Membership operations
        public async Task<bool> AddGroupMemberAsync(GroupMember member)
        {
            // Check if membership already exists
            var existingMember = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == member.GroupId && m.UserId == member.UserId);

            if (existingMember != null)
                return false;

            _context.GroupMembers.Add(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveGroupMemberAsync(string groupId, string userId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (member == null)
                return false;

            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateGroupMemberAdminStatusAsync(string groupId, string userId, bool isAdmin)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (member == null)
                return false;

            member.IsAdmin = isAdmin;
            await _context.SaveChangesAsync();
            return true;
        }

        // Task operations
        public async Task<List<TaskModel>> GetTasksByGroupIdAsync(string groupId)
        {
            // Get all group task associations for this group
            var groupTasks = await _context.GroupTasks
                .Where(gt => gt.GroupId == groupId)
                .Include(gt => gt.Task)
                .ToListAsync();

            // Extract and return the actual tasks
            return groupTasks
                .Select(gt => gt.Task)
                .Where(t => t != null)
                .ToList();
        }

        public async Task<bool> AddTaskToGroupAsync(GroupTask groupTask)
        {
            // Check if task is already in group
            var existingTask = await _context.GroupTasks
                .FirstOrDefaultAsync(t => t.GroupId == groupTask.GroupId && t.TaskId == groupTask.TaskId);

            if (existingTask != null)
                return false;

            _context.GroupTasks.Add(groupTask);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTaskFromGroupAsync(string groupId, string taskId)
        {
            var groupTask = await _context.GroupTasks
                .FirstOrDefaultAsync(t => t.GroupId == groupId && t.TaskId == taskId);

            if (groupTask == null)
                return false;
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null && task.IsRoot)
            {
                Console.WriteLine($"Cannot remove task {taskId} from group {groupId} because it is a root task.");
                throw new InvalidOperationException("Root tasks cannot be removed from groups.");
            }
            _context.GroupTasks.Remove(groupTask);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignTaskToUserAsync(string groupId, string taskId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get the GroupTask
                var groupTask = await _context.GroupTasks
                    .FirstOrDefaultAsync(t => t.GroupId == groupId && t.TaskId == taskId);

                if (groupTask == null)
                {
                    Console.WriteLine($"[GROUP_ASSIGNMENT] No task found");
                    return false;
                }

                // Check if user is a member
                var isMember = await _context.GroupMembers
                    .AnyAsync(m => m.GroupId == groupId && m.UserId == userId);

                if (!isMember)
                {
                    Console.WriteLine($"[GROUP_ASSIGNMENT] Not a member");
                    return false;
                }

                // Initialize if needed
                if (groupTask.AssignedUserIds == null)
                {
                    groupTask.AssignedUserIds = new List<string>();
                }

                // Add the user if not already assigned
                if (!groupTask.AssignedUserIds.Contains(userId))
                {
                    groupTask.AssignedUserIds.Add(userId);

                    // Save changes
                    _context.Entry(groupTask).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[GROUP_ASSIGNMENT] Error: {ex.Message}");
                return false;
            }
        }



        public async Task<bool> UnassignTaskFromUserAsync(string groupId, string taskId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get the GroupTask
                var groupTask = await _context.GroupTasks
                    .FirstOrDefaultAsync(t => t.GroupId == groupId && t.TaskId == taskId);

                if (groupTask == null)
                {
                    Console.WriteLine($"[GROUP_UNASSIGN] No task found");
                    return false;
                }

                // Initialize if needed
                if (groupTask.AssignedUserIds == null)
                {
                    groupTask.AssignedUserIds = new List<string>();
                }

                // Remove the user if assigned
                if (groupTask.AssignedUserIds.Contains(userId))
                {
                    groupTask.AssignedUserIds.Remove(userId);

                    // Save changes
                    _context.Entry(groupTask).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine($"[GROUP_UNASSIGN] User {userId} was not assigned to this task");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[GROUP_UNASSIGN] Error: {ex.Message}");
                return false;
            }
        }


    }
}
