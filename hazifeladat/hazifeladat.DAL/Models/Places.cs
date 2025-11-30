using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Store;
using hazifeladat.DAL.Models.Enums;


namespace hazifeladat.DAL.Models
{
    public class Places
    {
        public string Id { get; set; }
        public PlaceTypes Type { get; set; }
        public int Capicity { get; set; }
        public int PricePerNight { get; set; }
        public List<String>? Amenities { get; set; }
        public PlaceStatus Status { get; set; }
    }
}
