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
    public class UserRepository : IUserRepository
    {

        private readonly string _filePath;
        private List<User> _users = new List<User>();

        public UserRepository(string fileName = "User.json")
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
                    _users = new List<User>();
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
                    var data = JsonConvert.DeserializeObject<List<User>>(json);
                    _users = data ?? new List<User>();
                }

                return true;
            }
            catch
            {
                _users = new List<User>();
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
                    string json = JsonConvert.SerializeObject(_users, Formatting.Indented);
                    await writer.WriteAsync(json);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task AddAsync(User user)
        {
            if (user.Id.Equals(0))
            {
                int nextId = _users.Any() ? _users.Max(b => b.Id) + 1 : 1;
                user.Id = nextId;
            }

            _users.Add(user);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = _users.SingleOrDefault(b => b.Id == id);
            if (existing == null)
                return;

            _users.Remove(existing);
            await SaveAsync();
        }

        public Task<IReadOnlyList<User>> GetAllAsync()
        {
            IReadOnlyList<User> result = _users.ToList();
            return Task.FromResult(result);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            var booking = _users.SingleOrDefault(b => b.Id == id);
            return Task.FromResult(booking);
        }

        public async Task UpdateAsync(User user)
        {
            var existing = _users.SingleOrDefault(b => b.Id == user.Id);
            if (existing == null)
                return;

            int index = _users.IndexOf(existing);
            _users[index] = user;

            await SaveAsync();
        }


    }
}
