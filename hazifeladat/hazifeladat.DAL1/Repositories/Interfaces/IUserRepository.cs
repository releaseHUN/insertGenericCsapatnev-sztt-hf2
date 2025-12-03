using hazifeladat.DAL1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.DAL1.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> LoadAsync();
        Task<bool> SaveAsync();
        Task DeleteAsync(int id);
        Task AddAsync(User user);

        Task<IReadOnlyList<User>> GetAllAsync();
    }
}
