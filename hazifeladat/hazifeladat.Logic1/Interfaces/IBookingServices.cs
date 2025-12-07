using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Models.Enums;
using hazifeladat.Logic1.Dto;

namespace hazifeladat.Logic.Interfaces
{
    public interface IBookingServices
    {
        Task InitializeAsync();

        Task<IReadOnlyList<Booking>> GetAllBookingsAsync();
        Task<IReadOnlyList<Booking>> GetUserBookingsAsync(int userId);
        Task<IReadOnlyList<Booking>> GetPlaceBookingsAsync(int placeId);
        Task<IReadOnlyList<Booking>> GetBookingsInPeriodAsync(DateTime from, DateTime to);


        Task<Booking> CreateBookingForPlaceAsync(
            int userId,
            int placeId,
            DateTime arrival,
            DateTime departure,
            int numberOfGuests,
            string guestName);

        Task<Booking> CreateBookingForPlaceTypeAsync(
            int userId,
            PlaceTypes placeType,
            DateTime arrival,
            DateTime departure,
            int numberOfGuests,
            string guestName);

        Task<BookingModification> ModifyBookingAsync(
            int bookingId,
            DateTime newArrival,
            DateTime newDeparture,
            int newNumberOfGuests);

        Task<bool> CancelBookingAsync(int bookingId, int userId, UserRole userRole);

        Task<bool> IsPlaceAvailableAsync(
            int placeId,
            DateTime arrival,
            DateTime departure,
            int? ignoreBookingId = null);

        Task<IReadOnlyList<PlaceAvailabilityDto>> GetAvailabilityAsync(
            DateTime from,
            DateTime to,
            PlaceTypes? typeFilter = null,
            int? minCapacity = null);
    }


}

