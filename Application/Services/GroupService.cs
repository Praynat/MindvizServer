using Microsoft.EntityFrameworkCore;
using MindvizServer.Application.Interfaces;
using MindvizServer.Core.Interfaces;
using MindvizServer.Core.Models;

namespace MindvizServer.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;

        public GroupService(
            IGroupRepository groupRepository, 
            IUserRepository userRepository,
            ITaskRepository taskRepository)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _taskRepository = taskRepository;
        }

        // Group management
        public async Task<List<Group>> GetAllGroupsAsync()
        {
            return await _groupRepository.GetAllGroupsAsync();
        }

        public async Task<Group> GetGroupByIdAsync(string id, string userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(id);
            
            // Check if user is a member of the group
            if (group != null && group.Members.Any(m => m.UserId == userId))
            {
                return group;
            }
            
            return null; // Not found or not authorized
        }

        public async Task<Group> CreateGroupAsync(Group group, string creatorId)
        {
            // Ensure the creator ID is set
            group.CreatorId = creatorId;
            
            // Add the creator as an admin member
            group.Members.Add(new GroupMember
            {
                UserId = creatorId,
                IsAdmin = true,
                JoinedAt = DateTime.Now,
                InvitedBy = creatorId // Self-invited (creator)
            });
            
            return await _groupRepository.CreateGroupAsync(group);
        }

        public async Task<Group> UpdateGroupAsync(string id, Group group, string userId)
        {
            var existingGroup = await _groupRepository.GetGroupByIdAsync(id);
            
            // Check if user can manage the group
            if (existingGroup == null || !CanManageGroup(existingGroup, userId))
            {
                return null;
            }
            
            // Only update basic group properties, not members or tasks
            existingGroup.Name = group.Name;
            existingGroup.Description = group.Description;
            existingGroup.UpdatedAt = DateTime.Now;
            
            return await _groupRepository.UpdateGroupAsync(id, existingGroup);
        }

        public async Task<Group> DeleteGroupAsync(string id, string userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(id);
            
            // Check if user can manage the group (only creator or admin)
            if (group == null || !CanManageGroup(group, userId))
            {
                return null;
            }
            
            return await _groupRepository.DeleteGroupAsync(id);
        }

        public async Task<List<Group>> GetUserGroupsAsync(string userId)
        {
            return await _groupRepository.GetGroupsByUserIdAsync(userId);
        }

        // Membership management
        public async Task<bool> AddMemberByEmailAsync(string groupId, string email, bool isAdmin, string invitedByUserId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            
            // Check if inviter can manage members
            if (group == null || !CanManageMembers(group, invitedByUserId))
            {
                return false;
            }
            
            // Find user by email
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return false; // User not found
            }
            
            // Check if user is already a member
            if (group.Members.Any(m => m.UserId == user.Id))
            {
                return false; // Already a member
            }
            
            // Add new member
            var member = new GroupMember
            {
                GroupId = groupId,
                UserId = user.Id,
                IsAdmin = isAdmin,
                JoinedAt = DateTime.Now,
                InvitedBy = invitedByUserId
            };
            
            group.Members.Add(member);
            await _groupRepository.UpdateGroupAsync(groupId, group);
            
            return true;
        }

        public async Task<bool> RemoveMemberAsync(string groupId, string userId, string requestingUserId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            
            // Check if requester can manage members
            if (group == null || !CanManageMembers(group, requestingUserId))
            {
                return false;
            }
            
            // Cannot remove the creator
            if (userId == group.CreatorId)
            {
                return false;
            }
            
            // Find and remove the member
            var member = group.Members.FirstOrDefault(m => m.UserId == userId);
            if (member == null)
            {
                return false; // Not a member
            }
            
            return await _groupRepository.RemoveGroupMemberAsync(groupId, userId);
        }

        public async Task<bool> SetMemberAdminStatusAsync(string groupId, string userId, bool isAdmin, string requestingUserId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            
            // Check if requester can manage members
            if (group == null || !CanManageMembers(group, requestingUserId))
            {
                return false;
            }
            
            // Find the member
            var member = group.Members.FirstOrDefault(m => m.UserId == userId);
            if (member == null)
            {
                return false; // Not a member
            }
            
            return await _groupRepository.UpdateGroupMemberAdminStatusAsync(groupId, userId, isAdmin);
        }

        // Task management
        public async Task<List<TaskModel>> GetGroupTasksAsync(string groupId, string userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);

            // Check if user is a member of the group
            if (group == null || !group.Members.Any(m => m.UserId == userId))
            {
                return null; // Not found or not authorized
            }

            return await _groupRepository.GetTasksByGroupIdAsync(groupId);
        }

        public async Task<bool> AddTaskToGroupAsync(string groupId, string taskId, string userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            
            // Check if user can manage tasks
            if (group == null || !CanManageTasks(group, userId))
            {
                return false;
            }
            
            // Check if task exists
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return false;
            }
            
            // Create a new group task
            var groupTask = new GroupTask
            {
                GroupId = groupId,
                TaskId = taskId,
                CreatedBy = userId,
                AddedAt = DateTime.Now,
                AssignedUserIds = new List<string>()
            };
            
            return await _groupRepository.AddTaskToGroupAsync(groupTask);
        }

        public async Task<bool> RemoveTaskFromGroupAsync(string groupId, string taskId, string userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            
            // Check if user can manage tasks
            if (group == null || !CanManageTasks(group, userId))
            {
                return false;
            }
            
            return await _groupRepository.RemoveTaskFromGroupAsync(groupId, taskId);
        }

        public async Task<bool> AssignTaskToMemberAsync(string groupId, string taskId, string memberId, string requestingUserId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            
            // Check if requester can manage tasks
            if (group == null || !CanManageTasks(group, requestingUserId))
            {
                return false;
            }
            
            // Check if target is a member of the group
            if (!group.Members.Any(m => m.UserId == memberId))
            {
                return false;
            }
            
            return await _groupRepository.AssignTaskToUserAsync(groupId, taskId, memberId);
        }

        public async Task<bool> UnassignTaskFromMemberAsync(string groupId, string taskId, string memberId, string requestingUserId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            
            // Check if requester can manage tasks
            if (group == null || !CanManageTasks(group, requestingUserId))
            {
                return false;
            }
            
            return await _groupRepository.UnassignTaskFromUserAsync(groupId, taskId, memberId);
        }

        // Authorization checks
        public bool CanManageGroup(Group group, string userId)
        {
            // Only admins can manage the group
            return group.Members.Any(m => m.UserId == userId && m.IsAdmin);
        }

        public bool CanManageMembers(Group group, string userId)
        {
            // Only admins can manage members
            return group.Members.Any(m => m.UserId == userId && m.IsAdmin);
        }

        public bool CanManageTasks(Group group, string userId)
        {
            // Only admins can manage tasks
            return group.Members.Any(m => m.UserId == userId && m.IsAdmin);
        }
    }
}
