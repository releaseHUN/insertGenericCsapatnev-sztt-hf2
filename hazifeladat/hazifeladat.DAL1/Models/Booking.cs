using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hazifeladat.DAL1.Models.Enums;

namespace hazifeladat.DAL1.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int PlaceId { get; set; }
        public int UserId { get; set; }
        public string GuestName { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }

    }
}
