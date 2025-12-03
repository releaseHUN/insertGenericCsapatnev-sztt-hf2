using hazifeladat.DAL1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace hazifeladat.Logic1.Dto
{
    public class BookingModification
    {
        public bool Success { get; set; }
        public Booking? UpdatedBooking { get; set; }
        public string? ErrorMessage { get; set; }
        public IReadOnlyList<AlternativeOption> Alternatives { get; set; }
            = new List<AlternativeOption>();
    }

    public class AlternativeOption
    {
        public int? PlaceId { get; set; }
        public hazifeladat.DAL1.Models.Enums.PlaceTypes? PlaceType { get; set; }
        public System.DateTime SuggestedArrival { get; set; }
        public System.DateTime SuggestedDeparture { get; set; }
    }
}
