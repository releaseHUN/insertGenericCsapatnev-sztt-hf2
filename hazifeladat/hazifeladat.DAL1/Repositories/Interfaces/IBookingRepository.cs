using hazifeladat.DAL1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.DAL1.Repositories.Interfaces
{
    public interface IBookingRepository
    {

        Task<bool> LoadAsync();
        Task<bool> SaveAsync();
        Task DeleteAsync(int id);
        Task AddAsync(Booking booking);

        Task<IReadOnlyList<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task UpdateAsync(Booking booking);

    }
}
