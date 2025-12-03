using hazifeladat.DAL1.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.DAL1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public UserRole Role { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();


        public User(string name, string passwordHash, UserRole role)
        {
            Name = name;
            PasswordHash = passwordHash;
            Role = role;
        }

        public User() { }
    }
}
