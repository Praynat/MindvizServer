using MindvizServer.Core.Interfaces;
using MindvizServer.Core.Models;
using MindvizServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace MindvizServer.Infrastructure.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly MindvizDbContext _context;

        public UserRepository(MindvizDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (add logging service if necessary)
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return new List<User>(); // Return an empty list in case of failure
            }
        }

        public async Task<User?> CreateUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error creating user: {ex.Message}");
                return null; // Return null if creation fails
            }
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching user by ID {id}: {ex.Message}");
                return null; // Return null if fetch fails
            }
        }
        public async Task<User?> GetUserByNameAsync(string name)
        {
            try
            {
                return await _context.Users.FindAsync(name);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching user by name {name}: {ex.Message}");
                return null; // Return null if fetch fails
            }
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching user by email {email}: {ex.Message}");
                return null; // Return null if fetch fails
            }
        }

        public async Task<User?> UpdateUserAsync(string id, User user)
        {
            try
            {
                var userToUpdate = await _context.Users.FindAsync(id);
                if (userToUpdate == null)
                {
                    return null; // User not found
                }

                // Update properties
                userToUpdate.Name = user.Name;
                userToUpdate.Address = user.Address;
                userToUpdate.Image = user.Image;
                userToUpdate.Email = user.Email;
                userToUpdate.Phone = user.Phone;
                userToUpdate.Password = user.Password;
                userToUpdate.Role = user.Role;

                // Save changes
                await _context.SaveChangesAsync();
                return userToUpdate;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating user with ID {id}: {ex.Message}");
                return null; // Return null if update fails
            }
        }

        public async Task<User?> DeleteUserAsync(string id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return null; // User not found
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting user with ID {id}: {ex.Message}");
                return null; // Return null if deletion fails
            }
        }
    }
}
