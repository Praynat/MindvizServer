using MindvizServer.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace MindvizServer.Application.Utils
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
        public static string GenerateHashedPassword(string password, User user)
        {
            user.Password = "";
            return passwordHasher.HashPassword(user,password);
        }

        public static bool VerifyPassword(string password, string hashedPassword, User user)
        {
            user.Password = "";
            Console.WriteLine($"Password: {password}, HashedPassword: {hashedPassword}");

            PasswordVerificationResult result= passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
            if (result==PasswordVerificationResult.Failed)
            {
                Console.WriteLine("Password verification failed.");
                return false;
            }
            Console.WriteLine("Password verification succeeded.");
            return true;
        }
    }
}
