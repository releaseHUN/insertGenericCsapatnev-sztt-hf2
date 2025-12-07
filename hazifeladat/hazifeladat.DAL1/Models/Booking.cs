using hazifeladat.DAL1.Models.Enums;
using hazifeladat.Logic1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

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
        public float? TotalPrice { get; set; }

        public override string ToString()
        {
            string priceInfo = TotalPrice.HasValue 
                ? $" | Ár: {TotalPrice.Value.ToString("F0", CultureInfo.InvariantCulture)}" 
                : "";
            return $"| Foglalás ID: {BookingId,displayConfig.numFieldWidth} | Hely ID: {PlaceId,displayConfig.numFieldWidth} | Vendég név: {GuestName,displayConfig.nameFieldWidth} | Vendég ID: {UserId,displayConfig.numFieldWidth} | Férőhelyek száma: {NumberOfGuests,displayConfig.numFieldWidth} | {Arrival:yyyy-MM-dd} - {Departure:yyyy-MM-dd}{priceInfo} |";
        }
    }
}
