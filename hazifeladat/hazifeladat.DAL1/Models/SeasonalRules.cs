using hazifeladat.DAL1.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace hazifeladat.DAL1.Models
{
    public class SeasonalRules
    {
        private static int _nextId = 1;
        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public float Multiplier { get; set; }
        public ICollection<PlaceTypes>? AppliesToType { get; set; }
        public string? Name { get; set; }

        public SeasonalRules()
        {
            Id = _nextId++;
        }

    }
}
