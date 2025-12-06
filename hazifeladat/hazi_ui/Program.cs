using hazifeladat.DAL1.Repositories.Interfaces;
using hazifeladat.DAL1.Repositories.Repositories;
using hazifeladat.Logic.Interfaces;
using hazifeladat.Logic.Services;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        // repository-k inicializálása
        var bookingRepository = new BookingRepository("Bookings.json");
        var bookingLoaded = await bookingRepository.LoadAsync();
        var userRepository = new UserRepository("Users.json");
        var userLoaded = await userRepository.LoadAsync();
        var placesRepository = new PlacesRepository("Places.json");
        placesRepository.LoadAsync().Wait();
        BookingService bookingService = new BookingService(bookingRepository, userRepository, placesRepository);
        bookingService.InitializeAsync().Wait();

        // példa: összes foglalás kiírása
        var allBookings = await bookingService.GetAllBookingsAsync();
        foreach (var booking in allBookings)
        {
            Console.WriteLine(booking.ToString());
        }

        Console.ReadLine();
    }
}