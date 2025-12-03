using hazifeladat.DAL1.Models.Enums;
using hazifeladat.DAL1.Models;
using hazifeladat.Logic.Services;
using hazifeladat.Logic.Interfaces;

namespace hazifeladat.Tests;

[TestClass]
public class BookingServiceTest
{
    
    
    private InMemoryBookingRepository _bookingRepo = null!;
    private InMemoryUserRepository _userRepo = null!;
    private InMemoryPlacesRepository _placesRepo = null!;
    private IBookingServices _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _bookingRepo = new InMemoryBookingRepository();

        var users = new[]
        {
                new User
                {
                    Id = 1,
                    UserName = "tamas",
                    PasswordHash = "hash",
                    Role = UserRole.GUEST
                }
            };
        _userRepo = new InMemoryUserRepository(users);

        var places = new[]
        {
                new Places
                {
                    Id = 1,
                    Type = (PlaceTypes)2,
                    Capacity = 4,
                    PricePerNight = 10000m,
                    Status = PlaceStatus.AVAILABLE,
                    Amenities = new System.Collections.Generic.List<string> { "Áram", "Víz" }
                },
                new Places
                {
                    Id = 2,
                    Type = (PlaceTypes)2,
                    Capacity = 2,
                    PricePerNight = 8000m,
                    Status = PlaceStatus.AVAILABLE
                }
            };
        _placesRepo = new InMemoryPlacesRepository(places);

        _service = new BookingService(_bookingRepo, _userRepo, _placesRepo);
    }

    [TestMethod]
    public async Task CreateBookingForPlaceAsync_Creates_WhenFree()
    {
        var booking = await _service.CreateBookingForPlaceAsync(
            userId: 1,
            placeId: 1,
            arrival: new DateTime(2025, 6, 1),
            departure: new DateTime(2025, 6, 4),
            numberOfGuests: 2,
            guestName: "Tamas");

        Assert.IsNotNull(booking);
        Assert.AreNotEqual(0, booking.BookingId);
        Assert.AreEqual(1, booking.PlaceId);
        Assert.AreEqual(1, booking.UserId);

        var all = await _bookingRepo.GetAllAsync();
        Assert.AreEqual(1, all.Count);
    }

    [TestMethod]
    public async Task CreateBookingForPlaceAsync_Throws_WhenOverlapping()
    {
        await _bookingRepo.AddAsync(new Booking
        {
            UserId = 1,
            PlaceId = 1,
            GuestName = "Existing",
            NumberOfGuests = 2,
            Arrival = new DateTime(2025, 7, 1),
            Departure = new DateTime(2025, 7, 5)
        });

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
        {
            await _service.CreateBookingForPlaceAsync(
                1, 1,
                new DateTime(2025, 7, 3),
                new DateTime(2025, 7, 6),
                2,
                "New");
        });
    }

    [TestMethod]
    public async Task CreateBookingForPlaceTypeAsync_PicksFreePlace()
    {
        await _bookingRepo.AddAsync(new Booking
        {
            UserId = 1,
            PlaceId = 1,
            GuestName = "Existing",
            NumberOfGuests = 2,
            Arrival = new DateTime(2025, 8, 1),
            Departure = new DateTime(2025, 8, 5)
        });

        var booking = await _service.CreateBookingForPlaceTypeAsync(
            userId: 1,
            placeType: (PlaceTypes)2,
            arrival: new DateTime(2025, 8, 2),
            departure: new DateTime(2025, 8, 4),
            numberOfGuests: 2,
            guestName: "AutoGuest");

        Assert.AreEqual(2, booking.PlaceId, "A megadott idõszakra nincs szabad hely ebbõl a típusból.");
    }

    [TestMethod]
    public async Task CancelBookingAsync_Removes_Booking()
    {
        var booking = await _service.CreateBookingForPlaceAsync(
            1, 1,
            new DateTime(2025, 11, 1),
            new DateTime(2025, 11, 2),
            1,
            "CancelMe");

        bool ok = await _service.CancelBookingAsync(booking.BookingId);
        var all = await _bookingRepo.GetAllAsync();

        Assert.IsTrue(ok);
        Assert.AreEqual(0, all.Count);
    }
}
