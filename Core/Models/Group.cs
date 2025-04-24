using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MindvizServer.Core.Models.SubModels;

namespace MindvizServer.Core.Models
{
    public class Group
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Group name is required.")]
        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        // Creation metadata
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // The original creator (for reference only)
        [JsonPropertyName("creator_id")]
        public string? CreatorId { get; set; }

        // Collection of all group members - admin status is managed in the GroupMember entity
        public List<GroupMember> Members { get; set; } = new List<GroupMember>();

        // Tasks associated with this group
        public List<GroupTask> Tasks { get; set; } = new List<GroupTask>();

        // Helper method to check if a user is an admin in this group
        public bool IsUserAdmin(string userId)
        {
            return Members.Any(m => m.UserId == userId && m.IsAdmin);
        }

        // Helper method to get all admin user IDs
        public List<string> GetAdminIds()
        {
            return Members.Where(m => m.IsAdmin).Select(m => m.UserId).ToList();
        }
    }

    // Join entity between Group and User
    public class GroupMember
    {
        public string GroupId { get; set; }
        [JsonIgnore]
        public Group Group { get; set; }

        public string UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        // Admin status (single property instead of enum)
        public bool IsAdmin { get; set; } = false;

        // Metadata
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public string? InvitedBy { get; set; }
    }

    // Join entity between Group and Task
    public class GroupTask
    {
        public string GroupId { get; set; }
        [JsonIgnore]
        public Group Group { get; set; }

        public string TaskId { get; set; }
        [JsonIgnore]
        public TaskModel Task { get; set; }

        // The user who created/added this task to the group
        public string CreatedBy { get; set; }

        // When the task was added to the group
        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Users assigned to this task within the group
        public List<string> AssignedUserIds { get; set; } = new List<string>();

        // Optional due date specific to this group context
        public DateTime? DueDate { get; set; }
    }
}
