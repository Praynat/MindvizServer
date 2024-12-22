using MindvizServer.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MindvizServer.Infrastructure.Data
{
    public class MindvizDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public MindvizDbContext(DbContextOptions<MindvizDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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


            base.OnModelCreating(modelBuilder);
        }
    }
}
