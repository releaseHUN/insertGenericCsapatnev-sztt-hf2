using hazifeladat.DAL1.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace hazifeladat.DAL1.Models
{
    public class SeasonalRules
    {
        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal Multiplier { get; set; }
        public ICollection<PlaceTypes>? AppliesToType { get; set; }
        public string? Name { get; set; }
    }
}
