using hazifeladat.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.Logic.Interfaces
{
    public interface IUserService
    {
        ICollection<User> GetAllBookings();

        User GetBooking(string id);
    }
}
