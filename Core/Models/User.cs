using MindvizServer.Core.Models.SubModels;
using System.Text.Json.Serialization;

namespace MindvizServer.Core.Models
{
    public class User
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }=Guid.NewGuid().ToString();

        public Name Name { get; set; }
        public Address Address { get; set; }
        public Image Image { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }= UserRole.Normal;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();

        [JsonIgnore]
        public List<UserTask> UserTasks { get; set; } = new List<UserTask>();

        [JsonIgnore]
        public List<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();

    }
}
