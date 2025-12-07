using hazifeladat.DAL1.Models.Enums;
using hazifeladat.DAL1.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace hazifeladat.Logic1.Dto
{
    public class PlaceAvailabilityDto
    {
        public int PlaceId { get; set; }
        public PlaceTypes PlaceType { get; set; }
        public int? Capacity { get; set; }
        public PlaceStatus Status { get; set; }
        public float PricePerNight { get; set; }

        public bool IsAvailable { get; set; }

        public IReadOnlyList<Booking> OverlappingBookings { get; set; }
            = new List<Booking>();

        public override string ToString()
        {
            string priceStr = PricePerNight.ToString("F0", CultureInfo.InvariantCulture);
            return $"| ID: {PlaceId,displayConfig.numFieldWidth} | Típus: {PlaceType,displayConfig.typeFieldWidth} | Férőhely: {(Capacity == 0 || Capacity == null ? "-" : Capacity),displayConfig.numFieldWidth} | Ár/éj: {priceStr,6} | Elérhetőség: {(IsAvailable ? "Igen" : "Nem"),displayConfig.numFieldWidth} |";
        }
    }
}
