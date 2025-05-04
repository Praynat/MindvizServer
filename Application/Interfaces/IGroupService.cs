using MindvizServer.Core.Models;

namespace MindvizServer.Application.Interfaces
{
    public interface IGroupService
    {
        // Group management
        Task<List<Group>> GetAllGroupsAsync();
        Task<Group> GetGroupByIdAsync(string id, string userId);
        Task<Group> CreateGroupAsync(Group group, string creatorId);
        Task<Group> UpdateGroupAsync(string id, Group group, string userId);
        Task<Group> DeleteGroupAsync(string id, string userId);
        Task<List<Group>> GetUserGroupsAsync(string userId);
        
        // Membership management
        Task<bool> AddMemberByEmailAsync(string groupId, string email, bool isAdmin, string invitedByUserId);
        Task<bool> RemoveMemberAsync(string groupId, string userId, string requestingUserId);
        Task<bool> SetMemberAdminStatusAsync(string groupId, string userId, bool isAdmin, string requestingUserId);

        // Task management
        Task<List<TaskModel>> GetGroupTasksAsync(string groupId, string userId);

        Task<bool> AddTaskToGroupAsync(string groupId, string taskId, string userId);
        Task<bool> RemoveTaskFromGroupAsync(string groupId, string taskId, string userId);
        Task<bool> AssignTaskToMemberAsync(string groupId, string taskId, string memberId, string requestingUserId);
        Task<bool> UnassignTaskFromMemberAsync(string groupId, string taskId, string memberId, string requestingUserId);
        
        // Authorization checks
        bool CanManageGroup(Group group, string userId);
        bool CanManageMembers(Group group, string userId);
        bool CanManageTasks(Group group, string userId);
    }
}
