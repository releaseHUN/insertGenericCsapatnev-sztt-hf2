using hazifeladat.DAL1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.DAL1.Repositories.Interfaces
{
    public interface IPlacesRepository
    {
        Task<bool> LoadAsync();
        Task<bool> SaveAsync();
        Task DeleteAsync(int id);
        Task AddAsync(Places places);

        Task<IReadOnlyList<Places>> GetAllAsync();

        Task<Places> GetByIdAsync(int id);

        Task UpdateAsync(Places place);
    }
}

