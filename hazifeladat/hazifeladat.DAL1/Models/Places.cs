using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hazifeladat.DAL1.Models.Enums;


namespace hazifeladat.DAL1.Models
{
    public class Places
    {

        private static int _nextId = 1;
        public int Id { get; set; }
        public PlaceTypes Type { get; set; }
        public int ?Capacity { get; set; }
        public float PricePerNight { get; set; }
        public List<string>? Amenities { get; set; }
        public PlaceStatus Status { get; set; }



        public Places(
        PlaceTypes type,
        int? capacity,
        float pricePerNight,
        PlaceStatus status,
        List<string>? amenities
        ):this() // saját ID generálás
        {
            Type = type;
            Capacity = capacity;
            PricePerNight = pricePerNight;
            Status = status;

            Amenities = amenities;
        }

        public Places() { }

    }
}
