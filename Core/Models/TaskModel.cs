using MindvizServer.Core.Models.SubModels;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MindvizServer.Core.Models
{
    public class TaskModel
    {
        // Identification
        [JsonPropertyName("_id")]
        public string? Id { get; set; } 

        [Required(ErrorMessage = "Task name is required.")]
        [StringLength(100, ErrorMessage = "Task name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        // Organisation
        public List<string> ParentIds { get; set; } = new List<string>();
        public List<string> ChildrenIds { get; set; } = new List<string>();

        public TaskType Type { get; set; } = TaskType.Simple; //Simple or Complex or Category
        public bool IsRoot { get; set; } = false;
        // Statut et Progression
        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100.")]
        public int Progress { get; set; } = 0;

        // New property to track if a task is explicitly checked as completed
        public bool IsChecked { get; set; } = false;

        [Range(0, double.MaxValue, ErrorMessage = "Weight cannot be negative.")]
        public double Weight { get; set; } = 1;

        // Attributs Temporels
        public bool IsDeadline { get; set; } = false;
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Attributs de Fréquence
        public bool IsFrequency { get; set; } = false;
        public FrequencyType Frequency { get; set; } = FrequencyType.OneTime; // OneTime, Daily, Weekly, Monthly, Yearly, Custom
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<DateTime> NextOccurrences { get; set; } = new List<DateTime>();
        public WeekDays WeekDays { get; set; } = WeekDays.None; //separate different weekdays with |

        [Range(1, 31, ErrorMessage = "DayOfMonth must be between 1 and 31.")]
        public int? DayOfMonth { get; set; } // Specific day for monthly or yearly recurrence
        public MonthOfYear MonthOfYear { get; set; } = MonthOfYear.None;
        public int? FrequencyInterval { get; set; } // Interval for custom frequencies (e.g., every 3 months)

        // Relations et Contexte
        public List<string> Links { get; set; } = new List<string>();
        public List<string> Tags { get; set; } = new List<string>();

        public List<UserTask> UserTasks { get; set; } = new List<UserTask>();

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonIgnore]
        public List<GroupTask> GroupTasks { get; set; } = new List<GroupTask>();
    }
}
