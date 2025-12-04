using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.DAL1.Repositories.Repositories
{
    public class PlacesRepository : IPlacesRepository
    {

        private readonly string _filePath;
        private List<Places> _places = new List<Places>();

        public PlacesRepository(string fileName = "Places.json")
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
                    _places = new List<Places>();
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
                    var data = JsonConvert.DeserializeObject<List<Places>>(json);
                    _places = data ?? new List<Places>();
                }

                return true;
            }
            catch
            {
                _places = new List<Places>();
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
                    string json = JsonConvert.SerializeObject(_places, Formatting.Indented);
                    await writer.WriteAsync(json);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task AddAsync(Places places)
        {
            if (places.Id.Equals(0))
            {
                int nextId = _places.Any() ? _places.Max(b => b.Id) + 1 : 1;
                places.Id = nextId;
            }

            _places.Add(places);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = _places.SingleOrDefault(b => b.Id == id);
            if (existing == null)
                return;

            _places.Remove(existing);
            await SaveAsync();
        }

        public Task<IReadOnlyList<Places>> GetAllAsync()
        {
            IReadOnlyList<Places> result = _places.ToList();
            return Task.FromResult(result);
        }

        public Task<Places?> GetByIdAsync(int id)
        {
            var booking = _places.SingleOrDefault(b => b.Id == id);
            return Task.FromResult(booking);
        }

        public async Task UpdateAsync(Places place)
        {
            var existing = _places.SingleOrDefault(b => b.Id == place.Id);
            if (existing == null)
                return;

            int index = _places.IndexOf(existing);
            _places[index] = place;

            await SaveAsync();
        }
    }
}
