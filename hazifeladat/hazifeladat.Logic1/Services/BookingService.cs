using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Models.Enums;
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
    public class BookingService : IBookingServices
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPlacesRepository _placesRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IUserRepository userRepository,
            IPlacesRepository placesRepository)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _placesRepository = placesRepository;
        }

        public async Task InitializeAsync()
        {
            await _bookingRepository.LoadAsync();
            await _userRepository.LoadAsync();
            await _placesRepository.LoadAsync();
        }

        public async Task<IReadOnlyList<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<IReadOnlyList<Booking>> GetUserBookingsAsync(int userId)
        {
            var all = await _bookingRepository.GetAllAsync();
            return all.Where(b => b.UserId == userId).ToList();
        }

        public async Task<IReadOnlyList<Booking>> GetPlaceBookingsAsync(int placeId)
        {
            var all = await _bookingRepository.GetAllAsync();
            return all.Where(b => b.PlaceId == placeId).ToList();
        }

        public async Task<IReadOnlyList<Booking>> GetBookingsInPeriodAsync(DateTime from, DateTime to)
        {
            var all = await _bookingRepository.GetAllAsync();
            return all
                .Where(b => from < b.Departure && to > b.Arrival)
                .ToList();
        }

        public async Task<Booking> CreateBookingForPlaceAsync(
            int userId,
            int placeId,
            DateTime arrival,
            DateTime departure,
            int numberOfGuests,
            string guestName)
        {
            await ValidateBookingInputAsync(userId, placeId, arrival, departure, numberOfGuests, guestName);

            bool available = await IsPlaceAvailableAsync(placeId, arrival, departure);
            if (!available)
                throw new InvalidOperationException("A választott hely foglalt a megadott időszakban.");

            var booking = new Booking
            {
                BookingId = 0,
                UserId = userId,
                PlaceId = placeId,
                GuestName = guestName,
                NumberOfGuests = numberOfGuests,
                Arrival = arrival,
                Departure = departure
            };

            await _bookingRepository.AddAsync(booking);

            var bookingId = booking.BookingId;



            await _bookingRepository.AddAsync(booking);

            
            var place = await _placesRepository.GetByIdAsync(placeId);
            if (place != null)
            {
                place.Status = PlaceStatus.BOOKED;
                await _placesRepository.UpdateAsync(place);
            }

            return booking;
        }

        public async Task<Booking> CreateBookingForPlaceTypeAsync(
            int userId,
            PlaceTypes placeType,
            DateTime arrival,
            DateTime departure,
            int numberOfGuests,
            string guestName)
        {
            if (arrival.Date >= departure.Date)
                throw new ArgumentException("Az érkezésnek korábbinak kell lennie, mint a távozásnak.");

            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new InvalidOperationException("Felhasználó nem található.");

            var places = await _placesRepository.GetAllAsync();
            var candidates = places
                .Where(p => p.Type == placeType &&
                            (p.Capacity == null || p.Capacity.Value >= numberOfGuests) &&
                            p.Status == PlaceStatus.AVAILABLE)
                .ToList();

            if (!candidates.Any())
                throw new InvalidOperationException("A megadott típusból nincs megfelelő hely.");

            var allBookings = await _bookingRepository.GetAllAsync();
            var freePlace = candidates.FirstOrDefault(place =>
                !allBookings.Any(b =>
                    b.PlaceId == place.Id &&
                    arrival < b.Departure &&
                    departure > b.Arrival));

            if (freePlace == null)
                throw new InvalidOperationException("A megadott időszakra nincs szabad hely ebből a típusból.");

            return await CreateBookingForPlaceAsync(
                userId,
                freePlace.Id,
                arrival,
                departure,
                numberOfGuests,
                guestName);
        }

        public async Task<BookingModification> ModifyBookingAsync(
            int bookingId,
            DateTime newArrival,
            DateTime newDeparture,
            int newNumberOfGuests)
        {
            var result = new BookingModification();

            var existing = await _bookingRepository.GetByIdAsync(bookingId);
            if (existing == null)
            {
                result.Success = false;
                result.ErrorMessage = "A módosítandó foglalás nem található.";
                return result;
            }

            if (newArrival.Date >= newDeparture.Date)
            {
                result.Success = false;
                result.ErrorMessage = "Érvénytelen időszak.";
                return result;
            }

            var place = await _placesRepository.GetByIdAsync(existing.PlaceId)
                        ?? throw new InvalidOperationException("A foglaláshoz tartozó hely nem található.");

            if (place.Capacity.HasValue && newNumberOfGuests > place.Capacity.Value)
            {
                result.Success = false;
                result.ErrorMessage = "A vendégek száma meghaladja a hely kapacitását.";
                return result;
            }

            bool available = await IsPlaceAvailableAsync(existing.PlaceId, newArrival, newDeparture, bookingId);
            if (!available)
            {
                result.Success = false;
                result.ErrorMessage = "A megadott új időszak foglalt.";

                // nagyon egyszerű alternatíva példa: +1 nappal később próbáljuk
                var alt = new AlternativeOption
                {
                    PlaceId = existing.PlaceId,
                    PlaceType = place.Type,
                    SuggestedArrival = newArrival.AddDays(1),
                    SuggestedDeparture = newDeparture.AddDays(1)
                };
                result.Alternatives = new List<AlternativeOption> { alt };
                return result;
            }

            existing.Arrival = newArrival;
            existing.Departure = newDeparture;
            existing.NumberOfGuests = newNumberOfGuests;

            await _bookingRepository.UpdateAsync(existing);

            result.Success = true;
            result.UpdatedBooking = existing;
            return result;
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int userId, UserRole userRole)
        {
            var existing = await _bookingRepository.GetByIdAsync(bookingId);
            if (existing == null)
                return false;

            // Check if user is the booking owner or an admin
            if (userRole != UserRole.ADMIN && existing.UserId != userId)
                return false;

            await _bookingRepository.DeleteAsync(bookingId);
            return true;
        }

        public async Task<bool> IsPlaceAvailableAsync(
            int placeId,
            DateTime arrival,
            DateTime departure,
            int? ignoreBookingId = null)
        {
            var all = await _bookingRepository.GetAllAsync();

            var overlapping = all.Where(b =>
                    b.PlaceId == placeId &&
                    (ignoreBookingId == null || b.BookingId != ignoreBookingId.Value) &&
                    arrival < b.Departure &&
                    departure > b.Arrival)
                .ToList();

            return !overlapping.Any();
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

                var dto = new PlaceAvailabilityDto
                {
                    PlaceId = place.Id,
                    PlaceType = place.Type,
                    Capacity = place.Capacity,
                    Status = place.Status,
                    IsAvailable = place.Status == PlaceStatus.AVAILABLE && !overlapping.Any(),
                    OverlappingBookings = overlapping
                };

                result.Add(dto);
            }

            return result;
        }

        private async Task ValidateBookingInputAsync(
            int userId,
            int placeId,
            DateTime arrival,
            DateTime departure,
            int numberOfGuests,
            string guestName)
        {
            if (string.IsNullOrWhiteSpace(guestName))
                throw new ArgumentException("A vendég neve kötelező.", nameof(guestName));

            if (arrival.Date >= departure.Date)
                throw new ArgumentException("Az érkezésnek korábbinak kell lennie, mint a távozásnak.");

            if (numberOfGuests <= 0)
                throw new ArgumentException("A vendégek száma legyen pozitív.");

            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new InvalidOperationException("Felhasználó nem található.");

            var place = await _placesRepository.GetByIdAsync(placeId)
                        ?? throw new InvalidOperationException("Hely nem található.");

            if (place.Capacity.HasValue && numberOfGuests > place.Capacity.Value)
                throw new InvalidOperationException("A vendégek száma meghaladja a hely kapacitását.");

            if (place.Status != PlaceStatus.AVAILABLE)
                throw new InvalidOperationException("A hely jelenleg nem elérhető.");
        }
    }
}
