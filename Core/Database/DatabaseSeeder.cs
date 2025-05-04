using MindvizServer.Core.Models.SubModels;
using MindvizServer.Core.Models;
using MindvizServer.Infrastructure.Data;

namespace MindvizServer.Core.Database
{
    public class DatabaseSeeder
    {
        public static void SeedData(MindvizDbContext context)
        {
            if (!context.Tasks.Any())
            {
                // Root category (user's name)
                var rootCategory = new TaskModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Nathan",
                    Type = TaskType.Category,
                    Description = "Root category for all tasks",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsRoot = true
                };

                // Main categories
                var mainCategories = new List<TaskModel>
                {
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Well-being",
                        Type = TaskType.Category,
                        Description = "Activities related to physical, mental, and emotional health",
                        ParentIds = new List<string> { rootCategory.Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Work",
                        Type = TaskType.Category,
                        Description = "Professional responsibilities and career projects",
                        ParentIds = new List<string> { rootCategory.Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Family and Relationships",
                        Type = TaskType.Category,
                        Description = "Tasks concerning loved ones and social relationships",
                        ParentIds = new List<string> { rootCategory.Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Finances",
                        Type = TaskType.Category,
                        Description = "Managing expenses, savings, and administrative tasks related to money",
                        ParentIds = new List<string> { rootCategory.Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Hobbies and Passions",
                        Type = TaskType.Category,
                        Description = "Personal activities for enjoyment and personal development",
                        ParentIds = new List<string> { rootCategory.Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Learning and Development",
                        Type = TaskType.Category,
                        Description = "Goals related to education, skills, and learning",
                        ParentIds = new List<string> { rootCategory.Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Home and Daily Life",
                        Type = TaskType.Category,
                        Description = "Managing household tasks and daily responsibilities",
                        ParentIds = new List<string> { rootCategory.Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };

                // Example subcategories and tasks for each main category

                // Well-being
                var wellnessTasks = new List<TaskModel>
                {
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Physical Health",
                        Type = TaskType.Category,
                        Description = "Activities to maintain good physical fitness",
                        ParentIds = new List<string> { mainCategories[0].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Go Jogging",
                        Type = TaskType.Simple,
                        Description = "Run for 30 minutes in the park",
                        ParentIds = new List<string> { mainCategories[0].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsDeadline = true,
                        Deadline = DateTime.Now.AddDays(7),
                        Progress = 0
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Meditate for 15 Minutes",
                        Type = TaskType.Simple,
                        Description = "Meditation to reduce stress",
                        ParentIds = new List<string> { mainCategories[0].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Progress = 10
                    }
                };

                // Work
                var workTasks = new List<TaskModel>
                {
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Projects",
                        Type = TaskType.Category,
                        Description = "Managing professional projects",
                        ParentIds = new List<string> { mainCategories[1].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Finish Quarterly Report",
                        Type = TaskType.Simple,
                        Description = "Collect data and finalize the report",
                        ParentIds = new List<string> { mainCategories[1].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsDeadline = true,
                        Deadline = DateTime.Now.AddDays(3),
                        Progress = 30,
                        Tags = new List<string> { "Work", "Urgent" }
                    }
                };

                // Subcategories and tasks for "Family and Relationships"
                var familyTasks = new List<TaskModel>
                {
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Call Parents",
                        Type = TaskType.Simple,
                        Description = "Weekly call to check in with parents",
                        ParentIds = new List<string> { mainCategories[2].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Progress = 50
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Plan Family Outing",
                        Type = TaskType.Simple,
                        Description = "Organize a picnic with family this weekend",
                        ParentIds = new List<string> { mainCategories[2].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsDeadline = true,
                        Deadline = DateTime.Now.AddDays(3),
                        Progress = 0
                    }
                };

                // Subcategories and tasks for "Finances"
                var financeTasks = new List<TaskModel>
                {
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Pay Electricity Bill",
                        Type = TaskType.Simple,
                        Description = "Monthly payment for electricity",
                        ParentIds = new List<string> { mainCategories[3].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsDeadline = true,
                        Deadline = DateTime.Now.AddDays(10),
                        Progress = 0
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Budget Planning",
                        Type = TaskType.Simple,
                        Description = "Prepare a budget plan for next month",
                        ParentIds = new List<string> { mainCategories[3].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Progress = 20
                    }
                };

                // Subcategories and tasks for "Hobbies and Passions"
                var hobbyTasks = new List<TaskModel>
                {
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Learn Guitar",
                        Type = TaskType.Simple,
                        Description = "Practice guitar for 30 minutes daily",
                        ParentIds = new List<string> { mainCategories[4].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Progress = 15
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Start a Painting",
                        Type = TaskType.Simple,
                        Description = "Begin working on a landscape painting",
                        ParentIds = new List<string> { mainCategories[4].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Progress = 0
                    }
                };

                // Subcategories and tasks for "Learning and Development"
                var learningTasks = new List<TaskModel>
                {
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Complete SQL Course",
                        Type = TaskType.Simple,
                        Description = "Finish the online SQL course on Udemy",
                        ParentIds = new List<string> { mainCategories[5].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Progress = 80
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Read a Book",
                        Type = TaskType.Simple,
                        Description = "Read 'Atomic Habits' by James Clear",
                        ParentIds = new List<string> { mainCategories[5].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Progress = 50
                    }
                };

                // Subcategories and tasks for "Home and Daily Life"
                var homeTasks = new List<TaskModel>
                {
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Clean the House",
                        Type = TaskType.Simple,
                        Description = "Deep clean the house before the weekend",
                        ParentIds = new List<string> { mainCategories[6].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Progress = 0
                    },
                    new TaskModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Grocery Shopping",
                        Type = TaskType.Simple,
                        Description = "Buy groceries for the week",
                        ParentIds = new List<string> { mainCategories[6].Id },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Progress = 0
                    }
                };

                // Add everything to the database
                context.Tasks.Add(rootCategory);
                context.Tasks.AddRange(mainCategories);
                context.Tasks.AddRange(wellnessTasks);
                context.Tasks.AddRange(workTasks);
                context.Tasks.AddRange(familyTasks);
                context.Tasks.AddRange(financeTasks);
                context.Tasks.AddRange(hobbyTasks);
                context.Tasks.AddRange(learningTasks);
                context.Tasks.AddRange(homeTasks);



                // Linking children to parents by updating each parent's ChildrenIds.

                // Root category -> main categories
                rootCategory.ChildrenIds.AddRange(mainCategories.Select(m => m.Id));

                // mainCategories[0] ("Well-being") -> wellnessTasks
                mainCategories[0].ChildrenIds.AddRange(wellnessTasks.Select(w => w.Id));

                // mainCategories[1] ("Work") -> workTasks
                mainCategories[1].ChildrenIds.AddRange(workTasks.Select(w => w.Id));

                // mainCategories[2] ("Family and Relationships") -> familyTasks
                mainCategories[2].ChildrenIds.AddRange(familyTasks.Select(f => f.Id));

                // mainCategories[3] ("Finances") -> financeTasks
                mainCategories[3].ChildrenIds.AddRange(financeTasks.Select(f => f.Id));

                // mainCategories[4] ("Hobbies and Passions") -> hobbyTasks
                mainCategories[4].ChildrenIds.AddRange(hobbyTasks.Select(h => h.Id));

                // mainCategories[5] ("Learning and Development") -> learningTasks
                mainCategories[5].ChildrenIds.AddRange(learningTasks.Select(l => l.Id));

                // mainCategories[6] ("Home and Daily Life") -> homeTasks
                mainCategories[6].ChildrenIds.AddRange(homeTasks.Select(h => h.Id));
                // Save changes
                context.SaveChanges();
            }
        }
    }
}
