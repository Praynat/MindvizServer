using MindvizServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MindvizServer.Infrastructure.Data
{
    public class MindvizDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupTask> GroupTasks { get; set; }

        public MindvizDbContext(DbContextOptions<MindvizDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Existing UserTask configuration
            modelBuilder.Entity<UserTask>()
                .HasKey(ut => new { ut.UserId, ut.TaskId });

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTasks)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.Task)
                .WithMany(t => t.UserTasks)
                .HasForeignKey(ut => ut.TaskId);

            // Existing TaskModel property conversions
            modelBuilder.Entity<TaskModel>()
                .Property(t => t.Type)
                .HasConversion<int>();

            modelBuilder.Entity<TaskModel>()
                .Property(t => t.Frequency)
                .HasConversion<string>();

            modelBuilder.Entity<TaskModel>()
                .Property(t => t.WeekDays)
                .HasConversion<int>();

            modelBuilder.Entity<TaskModel>()
                .Property(t => t.MonthOfYear)
                .HasConversion<int>();

            // New GroupMember configuration (many-to-many between Group and User)
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.GroupId, gm.UserId });

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(gm => gm.UserId);

            // New GroupTask configuration (many-to-many between Group and Task)
            modelBuilder.Entity<GroupTask>()
                .HasKey(gt => new { gt.GroupId, gt.TaskId });

            modelBuilder.Entity<GroupTask>()
                .HasOne(gt => gt.Group)
                .WithMany(g => g.Tasks)
                .HasForeignKey(gt => gt.GroupId);

            modelBuilder.Entity<GroupTask>()
                .HasOne(gt => gt.Task)
                .WithMany(t => t.GroupTasks)
                .HasForeignKey(gt => gt.TaskId);

            // Store AssignedUserIds as JSON in GroupTask
            modelBuilder.Entity<GroupTask>()
                .Property(gt => gt.AssignedUserIds)
                .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
        );

            base.OnModelCreating(modelBuilder);
        }
    }
}
