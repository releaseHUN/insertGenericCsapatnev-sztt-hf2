using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Interfaces;
using hazifeladat.DAL1.Repositories.Repositories;
using hazifeladat.Logic.Interfaces;
using hazifeladat.Logic.Services;
using static hazi_ui.menuDisplays;
using static hazi_ui.userSession;
using static hazi_ui.helperFunctions;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        //segédváltozók
        bool running = true;

        //repository-k inicializálása
        var bookingRepository = new BookingRepository("Booking.json");
        var bookingLoaded = await bookingRepository.LoadAsync();
        var userRepository = new UserRepository("User.json");
        var userLoaded = await userRepository.LoadAsync();
        var placesRepository = new PlacesRepository("Places.json");
        var placesLoaded = await placesRepository.LoadAsync();
        if (!bookingLoaded || !userLoaded || !placesLoaded)
        {
            displayDividerLine(40, '=');
            Console.WriteLine("Hiba a fájlok betöltése során.");
            displayDividerLine(40, '=');
            return;
        }
        BookingService bookingService = new BookingService(bookingRepository, userRepository, placesRepository);
        bookingService.InitializeAsync().Wait();

        User user = new User();

        while (running)
        {
            displayLoginMenu();
            switch (readMenuChoice())
            {
                case 1:
                    user = await handleLogin(userRepository);
                    if (user != null)
                    {
                        await handleUserSession(bookingService, user);
                    }
                    break;
                case 2:
                    user = await handleRegistration(userRepository);
                    if (user != null)
                    {
                        await handleUserSession(bookingService, user);
                    }
                    break;
                case 0:
                    running = false;
                    break;
                default:
                    displayDividerLine(40, '=');
                    Console.WriteLine("Érvénytelen választás, próbálja újra.");
                    displayDividerLine(40, '=');
                    break;
            }
        } 
    }

    private static async Task<User?> handleRegistration(UserRepository users) {
        AuthService auth = new AuthService(users);
        auth.InitializeAsync().Wait();
        displayDividerLine(40, '-');
        Console.Write("Felhasználónév: ");
        string? userName = Console.ReadLine();
        displayDividerLine(40, '-');
        Console.Write("Teljes név: ");
        string? fullName = Console.ReadLine();
        displayDividerLine(40, '-');
        Console.Write("Jelszó: ");
        string? password = Console.ReadLine();
        try
        {
            var user = await auth.RegisterGuestAsync(userName ?? "", fullName ?? "", password ?? "");
            displayDividerLine(90, '=');
            Console.WriteLine($"Sikeres regisztráció, üdvözlünk {user.FullName}!");
            displayDividerLine(90, '=');
            return user;
        }
        catch (InvalidOperationException ex)
        {
            displayDividerLine(90, '=');
            Console.WriteLine($"Regisztráció sikertelen: {ex.Message}");
            displayDividerLine(90, '=');
            return null;
        }
    }

    private static async Task<User?> handleLogin(UserRepository users)
    {
        AuthService auth = new AuthService(users);
        auth.InitializeAsync().Wait();
        displayDividerLine(40, '-');
        Console.Write("Felhasználónév: ");
        string? userName = Console.ReadLine();
        displayDividerLine(40, '-');
        Console.Write("Jelszó: ");
        string? password = Console.ReadLine();
        var user = await auth.AuthenticateAsync(userName ?? "", password ?? "");
        if (user == null)
        {
            displayDividerLine(40, '=');
            Console.WriteLine("Hibás felhasználónév vagy jelszó.");
            displayDividerLine(40, '=');
            return null;
        }
        displayDividerLine(40, '=');
        Console.WriteLine($"Sikeres belépés, üdvözlünk {user.FullName}!");
        displayDividerLine(40, '=');
        return user;
    }
}