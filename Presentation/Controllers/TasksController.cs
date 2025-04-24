using MindvizServer.Application.Interfaces;
using MindvizServer.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MindvizServer.Presentation.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Authorize(Policy = "Must be logged")]
        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var tasks = await _taskService.GetAllTasksAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in GetAllTasks: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching tasks.");
            }
        }



        // Admin only: Fetch tasks for a specific user
        [Authorize(Roles = "Admin")]
        [HttpGet("user-tasks/{userId}")]
        public async Task<IActionResult> GetTasksByUserIdAsync(string userId)
        {
            try
            {
                string adminUserId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(adminUserId))
                {
                    return Unauthorized("Admin ID not found.");
                }

                var isAdmin = HttpContext.User.IsInRole("Admin");
                var tasks = await _taskService.GetTasksByUserIdAsync(adminUserId, userId, isAdmin);

                return Ok(tasks);
            }
           
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in GetTasksByUserIdAsync: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching tasks.");
            }
        }


        [Authorize(Policy = "Must be logged")]
        [HttpPost]
        public async Task<IActionResult> CreateTaskAsync(TaskModel task)
        {
            try
            {
                Console.WriteLine("Processing CreateTaskAsync...");
                string userIdClaim = HttpContext.User.FindFirst("_id")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Console.WriteLine("User ID not found in token.");
                    return Unauthorized("User ID is required.");
                }

                Console.WriteLine($"Extracted User ID: {userIdClaim}");
                var result = await _taskService.CreateTaskAsync(task, userIdClaim);

                if (result == null)
                {
                    Console.WriteLine("Task creation failed.");
                    return BadRequest("Failed to create task.");
                }

                Console.WriteLine("Task created successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in CreateTaskAsync: {ex.Message}");

                // Log the inner exception details, if present
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine($"Deeper Inner Exception: {ex.InnerException.InnerException.Message}");
                    }
                }

                // Optionally log the stack trace for debugging
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the task.");
            }
        }



        [Authorize(Policy = "Must be logged")]
        [HttpGet("task-id/{id}")]
        public async Task<IActionResult> GetTaskByIdAsync(string id)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var isAdmin = HttpContext.User.IsInRole("Admin");
                TaskModel? result = await _taskService.GetTaskByIdAsync(id, userId, isAdmin);

                if (result == null)
                {
                    return NotFound("Task not found or you do not have permission to view it.");
                }

                return Ok(result);
            }
            
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in GetTaskByIdAsync: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the task.");
            }
        }



        
        [Authorize(Policy = "Must be logged")]
        [HttpGet("task-name/{name}")]
        public async Task<IActionResult> GetTasksByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Task name cannot be null or empty.");
                }

                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var isAdmin = HttpContext.User.IsInRole("Admin");
                List<TaskModel> results = await _taskService.GetTasksByNameAsync(name, userId, isAdmin);

                if (results == null || results.Count == 0)
                {
                    return NotFound("No tasks found with the specified name.");
                }

                return Ok(results);
            }
            
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in GetTasksByNameAsync: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching tasks.");
            }
        }




        [Authorize(Policy = "Must be logged")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskAsync(string id, TaskModel task)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                var isAdmin = HttpContext.User.IsInRole("Admin");

                var result = await _taskService.UpdateTaskAsync(id, task, userId, isAdmin);

                if (result == null)
                {
                    return NotFound("Task not found or you do not have permission to update it.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in UpdateTaskAsync: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the task.");
            }
        }

        [Authorize(Policy = "Must be logged")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                var isAdmin = HttpContext.User.IsInRole("Admin");

                var result = await _taskService.DeleteTaskAsync(id, userId, isAdmin);

                if (result == null)
                {
                    return NotFound("Task not found or you do not have permission to delete it.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in DeleteTask: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the task.");
            }
        }



    }
}
