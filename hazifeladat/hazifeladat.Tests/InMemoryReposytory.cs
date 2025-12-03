// File: InMemoryRepositories.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Interfaces;

namespace hazifeladat.Tests
{
    internal class InMemoryBookingRepository : IBookingRepository
    {
        private readonly List<Booking> _bookings = new();

        public Task<bool> LoadAsync() => Task.FromResult(true);
        public Task<bool> SaveAsync() => Task.FromResult(true);

        public Task AddAsync(Booking booking)
        {
            if (booking.BookingId == 0)
            {
                int nextId = _bookings.Any() ? _bookings.Max(b => b.BookingId) + 1 : 1;
                booking.BookingId = nextId;
            }
            _bookings.Add(booking);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var existing = _bookings.SingleOrDefault(b => b.BookingId == id);
            if (existing != null)
                _bookings.Remove(existing);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Booking>> GetAllAsync()
            => Task.FromResult((IReadOnlyList<Booking>)_bookings.ToList());

        public Task<Booking?> GetByIdAsync(int id)
            => Task.FromResult(_bookings.SingleOrDefault(b => b.BookingId == id));

        public Task UpdateAsync(Booking booking)
        {
            var existing = _bookings.SingleOrDefault(b => b.BookingId == booking.BookingId);
            if (existing != null)
            {
                int index = _bookings.IndexOf(existing);
                _bookings[index] = booking;
            }
            return Task.CompletedTask;
        }
    }

    internal class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public InMemoryUserRepository(IEnumerable<User>? seed = null)
        {
            if (seed != null)
                _users.AddRange(seed);
        }

        public Task<bool> LoadAsync() => Task.FromResult(true);
        public Task<bool> SaveAsync() => Task.FromResult(true);

        public Task<IReadOnlyList<User>> GetAllAsync()
            => Task.FromResult((IReadOnlyList<User>)_users.ToList());

        public Task<User?> GetByIdAsync(int id)
            => Task.FromResult(_users.SingleOrDefault(u => u.Id == id));

        public Task AddAsync(User user)
        {
            if (user.Id == 0)
            {
                int nextId = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
                user.Id = nextId;
            }
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user)
        {
            var existing = _users.SingleOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                int index = _users.IndexOf(existing);
                _users[index] = user;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }

    internal class InMemoryPlacesRepository : IPlacesRepository
    {
        private readonly List<Places> _places = new();

        public InMemoryPlacesRepository(IEnumerable<Places>? seed = null)
        {
            if (seed != null)
                _places.AddRange(seed);
        }

        public Task<bool> LoadAsync() => Task.FromResult(true);
        public Task<bool> SaveAsync() => Task.FromResult(true);

        public Task<IReadOnlyList<Places>> GetAllAsync()
            => Task.FromResult((IReadOnlyList<Places>)_places.ToList());

        public Task<Places?> GetByIdAsync(int id)
            => Task.FromResult(_places.SingleOrDefault(p => p.Id == id));

        public Task AddAsync(Places place)
        {
            if (place.Id == 0)
            {
                int nextId = _places.Any() ? _places.Max(p => p.Id) + 1 : 1;
                place.Id = nextId;
            }
            _places.Add(place);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Places place)
        {
            var existing = _places.SingleOrDefault(p => p.Id == place.Id);
            if (existing != null)
            {
                int index = _places.IndexOf(existing);
                _places[index] = place;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var existing = _places.SingleOrDefault(p => p.Id == id);
            if (existing != null)
                _places.Remove(existing);
            return Task.CompletedTask;
        }
    }

    internal class InMemorySeasonRulesRepository : ISeasonRulesRepository
    {
        private readonly List<SeasonalRules> _rules = new();

        public InMemorySeasonRulesRepository(IEnumerable<SeasonalRules>? seed = null)
        {
            if (seed != null)
                _rules.AddRange(seed);
        }

        public Task<bool> LoadAsync() => Task.FromResult(true);
        public Task<bool> SaveAsync() => Task.FromResult(true);

        public Task<IReadOnlyList<SeasonalRules>> GetAllAsync()
            => Task.FromResult((IReadOnlyList<SeasonalRules>)_rules.ToList());

        public Task AddOrUpdateAsync(SeasonalRules rule)
        {
            if (rule.Id == 0)
            {
                rule.Id = _rules.Any() ? _rules.Max(r => r.Id) + 1 : 1;
                _rules.Add(rule);
            }
            else
            {
                var existing = _rules.SingleOrDefault(r => r.Id == rule.Id);
                if (existing == null)
                {
                    _rules.Add(rule);
                }
                else
                {
                    int index = _rules.IndexOf(existing);
                    _rules[index] = rule;
                }
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(int ruleId)
        {
            var existing = _rules.SingleOrDefault(p => p.Id == ruleId);
            if (existing != null)
                _rules.Remove(existing);
            return Task.CompletedTask;
        }
    }
}
