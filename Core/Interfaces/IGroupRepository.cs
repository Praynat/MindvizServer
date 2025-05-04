using MindvizServer.Core.Models;

namespace MindvizServer.Core.Interfaces
{
    public interface IGroupRepository
    {
        // Basic CRUD operations
        Task<List<Group>> GetAllGroupsAsync();
        Task<Group> GetGroupByIdAsync(string id);
        Task<Group> CreateGroupAsync(Group group);
        Task<Group> UpdateGroupAsync(string id, Group group);
        Task<Group> DeleteGroupAsync(string id);

        // Additional query methods
        Task<List<Group>> GetGroupsByUserIdAsync(string userId);
        Task<List<Group>> GetGroupsByCreatorIdAsync(string creatorId);

        // Membership operations
        Task<bool> AddGroupMemberAsync(GroupMember member);
        Task<bool> RemoveGroupMemberAsync(string groupId, string userId);
        Task<bool> UpdateGroupMemberAdminStatusAsync(string groupId, string userId, bool isAdmin);

        // Task operations
          Task<List<TaskModel>> GetTasksByGroupIdAsync(string groupId);

        Task<bool> AddTaskToGroupAsync(GroupTask groupTask);
        Task<bool> RemoveTaskFromGroupAsync(string groupId, string taskId);
        Task<bool> AssignTaskToUserAsync(string groupId, string taskId, string userId);
        Task<bool> UnassignTaskFromUserAsync(string groupId, string taskId, string userId);
    }
}
