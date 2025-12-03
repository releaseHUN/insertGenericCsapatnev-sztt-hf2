using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace hazifeladat.DAL1.Repositories.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _filePath;
        private List<Booking> _bookings = new List<Booking>();

        public BookingRepository(string fileName = "Booking.json")
        {
            var basePath = AppContext.BaseDirectory;
            _filePath = Path.Combine(basePath, "Data", fileName);
        }

        public async Task<bool> LoadAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _bookings = new List<Booking>();
                    return true;
                }

                using (var stream = new FileStream(
                    _filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    useAsync: true))
                using (var reader = new StreamReader(stream))
                {
                    string json = await reader.ReadToEndAsync();
                    var data = JsonConvert.DeserializeObject<List<Booking>>(json);
                    _bookings = data ?? new List<Booking>();
                }

                return true;
            }
            catch
            {
                _bookings = new List<Booking>();
                return false;
            }
        }


        public async Task<bool> SaveAsync()
        {
            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(
                    _filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    useAsync: true))
                using (var writer = new StreamWriter(stream))
                {
                    string json = JsonConvert.SerializeObject(_bookings, Formatting.Indented);
                    await writer.WriteAsync(json);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task AddAsync(Booking booking)
        {
            if (booking.BookingId == 0)
            {
                int nextId = _bookings.Any() ? _bookings.Max(b => b.BookingId) + 1 : 1;
                booking.BookingId = nextId;
            }

            _bookings.Add(booking);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = _bookings.SingleOrDefault(b => b.BookingId == id);
            if (existing == null)
                return;

            _bookings.Remove(existing);
            await SaveAsync();
        }

        public Task<IReadOnlyList<Booking>> GetAllAsync()
        {
            IReadOnlyList<Booking> result = _bookings.ToList();
            return Task.FromResult(result);
        }

        //public Task<Booking?> GetByIdAsync(int id)
        //{
        //    var booking = _bookings.SingleOrDefault(b => b.BookingId == id);
        //    return Task.FromResult(booking);
        //}

        //public async Task UpdateAsync(Booking booking)
        //{
        //    var existing = _bookings.SingleOrDefault(b => b.BookingId == booking.BookingId);
        //    if (existing == null)
        //        return;

        //    int index = _bookings.IndexOf(existing);
        //    _bookings[index] = booking;

        //    await SaveAsync();
        //}
    }
}
