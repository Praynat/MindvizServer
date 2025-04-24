using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MindvizServer.Application.Interfaces;
using MindvizServer.Core.Models;

namespace MindvizServer.Presentation.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = "Must be logged")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        // GET groups that the current user is a member of
        [HttpGet("my-groups")]
        public async Task<IActionResult> GetUserGroups()
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var groups = await _groupService.GetUserGroupsAsync(userId);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserGroups: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching groups.");
            }
        }

        // GET a specific group by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(string id)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var group = await _groupService.GetGroupByIdAsync(id, userId);
                if (group == null)
                {
                    return NotFound("Group not found or you are not a member.");
                }

                return Ok(group);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetGroupById: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the group.");
            }
        }

        // POST create a new group
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] Group group)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var createdGroup = await _groupService.CreateGroupAsync(group, userId);
                return CreatedAtAction(nameof(GetGroupById), new { id = createdGroup.Id }, createdGroup);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateGroup: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the group.");
            }
        }

        // PUT update group details
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(string id, [FromBody] Group group)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var updatedGroup = await _groupService.UpdateGroupAsync(id, group, userId);
                if (updatedGroup == null)
                {
                    return NotFound("Group not found or you don't have permission to update it.");
                }

                return Ok(updatedGroup);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateGroup: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the group.");
            }
        }

        // DELETE a group
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var deletedGroup = await _groupService.DeleteGroupAsync(id, userId);
                if (deletedGroup == null)
                {
                    return NotFound("Group not found or you don't have permission to delete it.");
                }

                return Ok(deletedGroup);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteGroup: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the group.");
            }
        }

        // POST add a member to a group
        [HttpPost("{groupId}/members")]
        public async Task<IActionResult> AddMember(string groupId, [FromBody] AddMemberRequest request)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var result = await _groupService.AddMemberByEmailAsync(groupId, request.Email, request.IsAdmin, userId);
                if (!result)
                {
                    return BadRequest("Failed to add member. User may not exist, already be a member, or you don't have permission.");
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddMember: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding a member.");
            }
        }

        // DELETE remove a member from a group
        [HttpDelete("{groupId}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember(string groupId, string memberId)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var result = await _groupService.RemoveMemberAsync(groupId, memberId, userId);
                if (!result)
                {
                    return BadRequest("Failed to remove member. User may not be a member or you don't have permission.");
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RemoveMember: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing a member.");
            }
        }

        // PUT update a member's admin status
        [HttpPut("{groupId}/members/{memberId}/admin")]
        public async Task<IActionResult> SetMemberAdminStatus(string groupId, string memberId, [FromBody] SetAdminRequest request)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var result = await _groupService.SetMemberAdminStatusAsync(groupId, memberId, request.IsAdmin, userId);
                if (!result)
                {
                    return BadRequest("Failed to update admin status. User may not be a member or you don't have permission.");
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetMemberAdminStatus: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating admin status.");
            }
        }

        // POST add a task to a group
        [HttpPost("{groupId}/tasks/{taskId}")]
        public async Task<IActionResult> AddTaskToGroup(string groupId, string taskId)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var result = await _groupService.AddTaskToGroupAsync(groupId, taskId, userId);
                if (!result)
                {
                    return BadRequest("Failed to add task to group. Task may not exist or you don't have permission.");
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddTaskToGroup: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding a task to the group.");
            }
        }

        // DELETE remove a task from a group
        [HttpDelete("{groupId}/tasks/{taskId}")]
        public async Task<IActionResult> RemoveTaskFromGroup(string groupId, string taskId)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var result = await _groupService.RemoveTaskFromGroupAsync(groupId, taskId, userId);
                if (!result)
                {
                    return BadRequest("Failed to remove task from group. Task may not exist in this group or you don't have permission.");
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RemoveTaskFromGroup: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing a task from the group.");
            }
        }

        // POST assign a task to a group member
        [HttpPost("{groupId}/tasks/{taskId}/assign/{memberId}")]
        public async Task<IActionResult> AssignTaskToMember(string groupId, string taskId, string memberId)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var result = await _groupService.AssignTaskToMemberAsync(groupId, taskId, memberId, userId);
                if (!result)
                {
                    return BadRequest("Failed to assign task. User may not be a member of this group, task may not exist, or you don't have permission.");
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AssignTaskToMember: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while assigning a task.");
            }
        }

        // DELETE unassign a task from a group member
        [HttpDelete("{groupId}/tasks/{taskId}/assign/{memberId}")]
        public async Task<IActionResult> UnassignTaskFromMember(string groupId, string taskId, string memberId)
        {
            try
            {
                string userId = HttpContext.User.FindFirst("_id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var result = await _groupService.UnassignTaskFromMemberAsync(groupId, taskId, memberId, userId);
                if (!result)
                {
                    return BadRequest("Failed to unassign task. Task may not be assigned to this user or you don't have permission.");
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UnassignTaskFromMember: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while unassigning a task.");
            }
        }
    }

    // Request models for the API endpoints
    public class AddMemberRequest
    {
        public string Email { get; set; }
        public bool IsAdmin { get; set; } = false;
    }

    public class SetAdminRequest
    {
        public bool IsAdmin { get; set; }
    }
}
