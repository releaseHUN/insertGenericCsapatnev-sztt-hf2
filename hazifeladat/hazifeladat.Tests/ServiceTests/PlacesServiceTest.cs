// File: PlaceServiceTests.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Models.Enums;
using hazifeladat.Logic.Interfaces;
using hazifeladat.Logic.Services;
using hazifeladat.Logic1.Interfaces;
using hazifeladat.Logic1.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace hazifeladat.Tests
{
    [TestClass]
    public class PlaceServiceTests
    {
        private InMemoryPlacesRepository _placesRepo = null!;
        private InMemoryBookingRepository _bookingRepo = null!;
        private IPlacesService _service = null!;

        [TestInitialize]
        public void Setup()
        {


            var places = new[]
            {
                new Places
                {
                    
                    Type = (PlaceTypes)0,
                    Capacity = 4,
                    PricePerNight = 500,
                    Status = PlaceStatus.AVAILABLE,
                    Amenities = new List<string>()
                },
                new Places
                {
                    
                    Type = (PlaceTypes)2,
                    Capacity = 2,
                    PricePerNight = 10000,
                    Status = PlaceStatus.AVAILABLE,
                    Amenities = new List<string> { "valami" }
                }
            };

            _placesRepo = new InMemoryPlacesRepository(places);
            _bookingRepo = new InMemoryBookingRepository();
            _service = new PlacesService(_placesRepo, _bookingRepo);
        }

        [TestMethod]
        public async Task DeletePlaceAsync_Fails_IfActiveBookingExists()
        {
            await _bookingRepo.AddAsync(new Booking
            {
                UserId = 1,
                PlaceId = 1,
                GuestName = "X",
                NumberOfGuests = 2,
                Arrival = DateTime.Now,
                Departure = DateTime.Now.AddDays(2)
            });

            bool ok = await _service.DeletePlaceAsync(1);
            Assert.IsFalse(ok);
        }

        [TestMethod]
        public async Task DeletePlaceAsync_Succeeds_IfNoActiveBooking()
        {
            bool ok = await _service.DeletePlaceAsync(2);
            Assert.IsTrue(ok);

            var all = await _placesRepo.GetAllAsync();
            Assert.IsFalse(all.Any(p => p.Id == 2));
        }

       
    }
}
