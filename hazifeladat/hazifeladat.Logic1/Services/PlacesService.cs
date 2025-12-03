using hazifeladat.DAL1.Models.Enums;
using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Interfaces;
using hazifeladat.Logic.Interfaces;
using hazifeladat.Logic1.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.Logic.Services
{
    public class PlacesService : IPlacesService
    {
        private readonly IPlacesRepository _placesRepository;
        private readonly IBookingRepository _bookingRepository;

        public PlacesService(IPlacesRepository placesRepository, IBookingRepository bookingRepository)
        {
            _placesRepository = placesRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task InitializeAsync()
        {
            await _placesRepository.LoadAsync();
            await _bookingRepository.LoadAsync();
        }

        public async Task<IReadOnlyList<Places>> GetAllPlacesAsync()
        {
            return await _placesRepository.GetAllAsync();
        }

        public async Task<Places?> GetPlaceByIdAsync(int id)
        {
            return await _placesRepository.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<Places>> SearchPlacesAsync(
            PlaceTypes? type = null,
            int? minCapacity = null,
            int? maxCapacity = null,
            IEnumerable<string>? requiredAmenities = null,
            PlaceStatus? status = null)
        {
            var places = await _placesRepository.GetAllAsync();
            var query = places.AsQueryable();

            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);

            if (minCapacity.HasValue)
                query = query.Where(p => !p.Capacity.HasValue || p.Capacity.Value >= minCapacity.Value);

            if (maxCapacity.HasValue)
                query = query.Where(p => !p.Capacity.HasValue || p.Capacity.Value <= maxCapacity.Value);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            if (requiredAmenities != null)
            {
                var reqList = requiredAmenities.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
                if (reqList.Any())
                {
                    query = query.Where(p =>
                        p.Amenities != null &&
                        reqList.All(r => p.Amenities.Contains(r)));
                }
            }

            return query.ToList();
        }

        public async Task<Places> CreatePlaceAsync(Places place)
        {
            // feltételezzük, hogy a repo ad ID-t, ha 0
            await _placesRepository.AddAsync(place);
            var all = await _placesRepository.GetAllAsync();
            return all.OrderByDescending(p => p.Id).First();
        }

        public async Task<bool> UpdatePlaceAsync(Places place)
        {
            var existing = await _placesRepository.GetByIdAsync(place.Id);
            if (existing == null)
                return false;

            await _placesRepository.UpdateAsync(place);
            return true;
        }

        public async Task<bool> DeletePlaceAsync(int placeId)
        {
            var bookings = await _bookingRepository.GetAllAsync();
            bool hasActiveBooking = bookings.Any(b =>
                b.PlaceId == placeId &&
                b.Departure > DateTime.Now);   // egyszerű "aktív" definíció

            if (hasActiveBooking)
                return false;

            await _placesRepository.DeleteAsync(placeId);
            return true;
        }

        public async Task<bool> SetPlaceStatusAsync(int placeId, PlaceStatus status)
        {
            var place = await _placesRepository.GetByIdAsync(placeId);
            if (place == null)
                return false;

            place.Status = status;
            await _placesRepository.UpdateAsync(place);
            return true;
        }

        public async Task<IReadOnlyList<PlaceAvailabilityDto>> GetAvailabilityAsync(
            DateTime from,
            DateTime to,
            PlaceTypes? typeFilter = null,
            int? minCapacity = null)
        {
            var places = await _placesRepository.GetAllAsync();
            var bookings = await _bookingRepository.GetAllAsync();

            if (typeFilter.HasValue)
                places = places.Where(p => p.Type == typeFilter.Value).ToList();

            if (minCapacity.HasValue)
                places = places.Where(p => !p.Capacity.HasValue || p.Capacity.Value >= minCapacity.Value).ToList();

            var result = new List<PlaceAvailabilityDto>();

            foreach (var place in places)
            {
                var overlapping = bookings
                    .Where(b => b.PlaceId == place.Id &&
                                from < b.Departure &&
                                to > b.Arrival)
                    .ToList();

                result.Add(new PlaceAvailabilityDto
                {
                    PlaceId = place.Id,
                    PlaceType = place.Type,
                    Capacity = place.Capacity,
                    Status = place.Status,
                    IsAvailable = place.Status == PlaceStatus.AVAILABLE && !overlapping.Any(),
                    OverlappingBookings = overlapping
                });
            }

            return result;
        }
    }
}
