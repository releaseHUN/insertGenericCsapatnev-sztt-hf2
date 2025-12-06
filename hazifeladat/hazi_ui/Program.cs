using hazifeladat.DAL1.Repositories.Interfaces;
using hazifeladat.DAL1.Repositories.Repositories;
using hazifeladat.Logic.Interfaces;
using hazifeladat.Logic.Services;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        // repository-k inicializálása
        var bookingRepository = new BookingRepository("bookings.json");
        var userRepository = new UserRepository("users.json");
        var placesRepository = new PlacesRepository("places.json");
        BookingService bookingService = new BookingService(bookingRepository, userRepository, placesRepository);
        bookingService.InitializeAsync().Wait();

        Console.ReadLine();
    }
}