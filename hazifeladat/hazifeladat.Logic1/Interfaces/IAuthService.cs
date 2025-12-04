using hazifeladat.DAL1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.Logic.Interfaces
{
    public interface IAuthService
    {
        Task InitializeAsync();
        Task<User?> AuthenticateAsync(string userName, string password);
        Task<User> RegisterGuestAsync(string userName, string fullName, string password);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}
