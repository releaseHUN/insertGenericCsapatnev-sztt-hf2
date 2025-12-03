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
        public string UserName { get; set; }
        public string FullName { get; set; }

        public UserRole Role { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();


        public User(string userName, string fullName, string passwordHash, UserRole role)
        {
            UserName = userName;
            FullName = fullName;
            PasswordHash = passwordHash;
            Role = role;
        }

        public User() { }
    }
}
