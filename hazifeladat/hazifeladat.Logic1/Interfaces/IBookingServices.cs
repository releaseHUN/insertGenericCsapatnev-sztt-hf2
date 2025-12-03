using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hazifeladat.DAL1.Models;

namespace hazifeladat.Logic.Interfaces
{
    public interface IBookingServices
    {
        ICollection<Booking> GetAllBookings();

        Booking GetBooking(string id);

        bool MakeBooking(User id, Places Placesid);

        bool CancelBooking(User id, Booking bookingId);

        bool UpdateBooking(User id, Booking bookingId);

        int CalculatePrice(Places placesId);



        
    }
}
