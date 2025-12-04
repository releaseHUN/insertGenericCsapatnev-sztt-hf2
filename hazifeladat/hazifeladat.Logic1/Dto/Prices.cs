using hazifeladat.DAL1.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace hazifeladat.Logic1.Dto
{
    public class PriceQuote
    {
        public decimal TotalPrice { get; set; }
        public decimal PricePerNight { get; set; }
        public int Nights { get; set; }
        public string? Description { get; set; }
    }

    public class SeasonRule
    {
        public int Id { get; set; }
        public DateTime From { get; set; }   // inclusive
        public DateTime To { get; set; }     // exclusive
        public decimal Multiplier { get; set; } = 1.0m; // pl. 1.2 = +20%
        public PlaceTypes? AppliesToType { get; set; }  // null = minden típus
        public string? Name { get; set; }
    }
}
