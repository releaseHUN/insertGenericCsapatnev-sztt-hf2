using hazifeladat.DAL1.Models.Enums;
using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using hazifeladat.Logic.Interfaces;
using hazifeladat.DAL1.Repositories.Repositories;

namespace hazifeladat.Logic.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task InitializeAsync()
        {
            await _userRepository.LoadAsync();
        }

        public async Task<User?> AuthenticateAsync(string userName, string password)
        {
            
            var users = await _userRepository.GetAllAsync();
            var user = users.SingleOrDefault(u =>
                string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return null;

            string hash = HashPassword(password);
            if (user.PasswordHash != hash)
                return null;

            return user;
        }

        public async Task<User> RegisterGuestAsync(string userName, string fullName, string password)
        {
            
            var users = await _userRepository.GetAllAsync();

            if (users.Any(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Már létezik ilyen felhasználónév.");

            string hash = HashPassword(password);

            var user = new User(userName, fullName, hash, UserRole.GUEST);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            string oldHash = HashPassword(oldPassword);
            if (user.PasswordHash != oldHash)
                return false;

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();
            return true;
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
