using hazifeladat.DAL1.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace hazifeladat.Logic1.Dto
{
    public class PriceQuote
    {
        public float TotalPrice { get; set; }
        public float PricePerNight { get; set; }
        public int Nights { get; set; }
        public string? Description { get; set; }
    }
}
