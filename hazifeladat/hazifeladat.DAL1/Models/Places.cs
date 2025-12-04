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
        public int Id { get; set; }
        public PlaceTypes Type { get; set; }
        public int ?Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public List<String>? Amenities { get; set; }
        public PlaceStatus Status { get; set; }
    }
}
