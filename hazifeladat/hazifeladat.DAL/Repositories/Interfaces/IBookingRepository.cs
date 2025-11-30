using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.DAL.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        bool SaveBooking();
        bool LoadBooking();
    }
}
