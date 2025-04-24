using Microsoft.EntityFrameworkCore;
using MindvizServer.Core.Interfaces;
using MindvizServer.Core.Models;
using MindvizServer.Infrastructure.Data;

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

            _context.GroupTasks.Remove(groupTask);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignTaskToUserAsync(string groupId, string taskId, string userId)
        {
            var groupTask = await _context.GroupTasks
                .FirstOrDefaultAsync(t => t.GroupId == groupId && t.TaskId == taskId);

            if (groupTask == null)
                return false;

            // Check if user is a member of the group
            var isMember = await _context.GroupMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (!isMember)
                return false;

            if (!groupTask.AssignedUserIds.Contains(userId))
            {
                groupTask.AssignedUserIds.Add(userId);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> UnassignTaskFromUserAsync(string groupId, string taskId, string userId)
        {
            var groupTask = await _context.GroupTasks
                .FirstOrDefaultAsync(t => t.GroupId == groupId && t.TaskId == taskId);

            if (groupTask == null)
                return false;

            if (groupTask.AssignedUserIds.Contains(userId))
            {
                groupTask.AssignedUserIds.Remove(userId);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
