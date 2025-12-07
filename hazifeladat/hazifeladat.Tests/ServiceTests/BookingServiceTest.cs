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
                    Id= 1,
                    Type = (PlaceTypes)2,
                    Capacity = 4,
                    PricePerNight = 10000,
                    Status = PlaceStatus.AVAILABLE,
                    Amenities = new System.Collections.Generic.List<string> { "Áram", "Víz" }
                },
                new Places
                {
                    Id = 2,
                    Type = (PlaceTypes)2,
                    Capacity = 2,
                    PricePerNight = 8000,
                    Status = PlaceStatus.AVAILABLE
                }
            };
        _placesRepo = new InMemoryPlacesRepository(places);

        _service = new BookingService(_bookingRepo, _userRepo, _placesRepo);
    }

    

    [TestMethod]
    public async Task CreateBookingForPlaceAsync_Throws_WhenOverlapping()
    {
        await _bookingRepo.AddAsync(new Booking
        {
            UserId = 1,
            PlaceId = 1,
            GuestName = "Existing",
            NumberOfGuests = 5,
            Arrival = new DateTime(2025, 7, 1),
            Departure = new DateTime(2025, 7, 5)
        });

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
        {
            await _service.CreateBookingForPlaceAsync(
                1, 
                1,
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
    public async Task CreateBookingForPlaceAsync_Throws_WhenGuestCountExceedsCapacity()
    {

        var test = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
        {
            await _service.CreateBookingForPlaceAsync(
                userId: 1,
                placeId: 2,
                arrival: new DateTime(2025, 9, 1),
                departure: new DateTime(2025, 9, 3),
                numberOfGuests: 3,  
                guestName: "TooMany");
        });

        Assert.AreEqual("A vendégek száma meghaladja a hely kapacitását.", test.Message);
    }



}
