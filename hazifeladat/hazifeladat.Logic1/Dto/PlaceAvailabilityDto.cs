using hazifeladat.DAL1.Models.Enums;
using hazifeladat.DAL1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace hazifeladat.Logic1.Dto
{
    public class PlaceAvailabilityDto
    {
        public int PlaceId { get; set; }
        public PlaceTypes PlaceType { get; set; }
        public int? Capacity { get; set; }
        public PlaceStatus Status { get; set; }

        public bool IsAvailable { get; set; }

        public IReadOnlyList<Booking> OverlappingBookings { get; set; }
            = new List<Booking>();

        public override string ToString()
        {
            return $"| ID: {PlaceId,displayConfig.numFieldWidth} | Típus: {PlaceType,displayConfig.typeFieldWidth} | Férőhely: {Capacity,displayConfig.numFieldWidth} | Elérhetőség: {(IsAvailable ? "Igen" : "Nem"),displayConfig.numFieldWidth} |";
        }
    }
}
