using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Repositories;
using hazifeladat.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static hazi_ui.menuDisplays;
using static hazi_ui.helperFunctions;

namespace hazi_ui
{
    internal class userSession
    {
        public static async Task handleUserSession(BookingService bookingService, User user)
        {
            bool sessionActive = true;
            while (sessionActive)
            {
                displayMenu(user);
                int choice = readMenuChoice();
                if (user.Role == hazifeladat.DAL1.Models.Enums.UserRole.ADMIN)
                {
                    switch (choice)
                    {
                        case 1:
                            await displayAllBookings(bookingService);
                            break;
                        case 2:
                            await handleAdminManageBooking(bookingService, user);
                            break;
                        case 3:
                            await handleDeleteBooking(bookingService, user);
                            break;
                        case 4:
                            await displayPlaces(bookingService);
                            break;
                        case 5:
                            await handleAddPlaceNotImplemented();
                            break;
                        case 0:
                            sessionActive = false;
                            break;
                        default:
                            displayDividerLine(40, '=');
                            Console.WriteLine("Érvénytelen választás, próbálja újra.");
                            displayDividerLine(40, '=');
                            break;
                    }
                }
                else if (user.Role == hazifeladat.DAL1.Models.Enums.UserRole.GUEST)
                {
                    switch (choice)
                    {
                        case 1:
                            await handleCreateBooking(bookingService, user);
                            break;
                        case 2:
                            await displayUserBookings(bookingService, user);
                            break;
                        case 3:
                            await handleModifyBooking(bookingService, user);
                            break;
                        case 4:
                            await handleDeleteBooking(bookingService, user);
                            break;
                        case 5:
                            await displayPlaces(bookingService);
                            break;
                        case 0:
                            sessionActive = false;
                            break;
                        default:
                            displayDividerLine(40, '=');
                            Console.WriteLine("Érvénytelen választás, próbáld újra.");
                            displayDividerLine(40, '=');
                            break;
                    }
                }
            }
        }

        private static async Task handleCreateBooking(BookingService bookingService, User user)
        {
            displayDividerLine(90, '=');
            Console.WriteLine("Foglalás indítása — adja meg a szükséges adatokat.");
            Console.Write("Szeretne konkrét helyet választani? (i = igen, t = típus szerint / üres = típus szerint): ");
            string? mode = Console.ReadLine()?.Trim().ToLower();

            int? placeId = null;
            hazifeladat.DAL1.Models.Enums.PlaceTypes? placeType = null;

            if (mode == "i" || mode == "igen")
            {
                displayDividerLine(90, '-');
                Console.Write("Adja meg a hely azonosítóját: ");
                string? placeIdStr = Console.ReadLine();
                if (!int.TryParse(placeIdStr, out int parsedPlaceId))
                {
                    displayDividerLine(90, '-');
                    Console.WriteLine("Érvénytelen hely azonosító.");
                    displayDividerLine(90, '=');
                    return;
                }
                placeId = parsedPlaceId;
            }
            else
            {
                displayDividerLine(90, '-');
                Console.WriteLine("Válasszon helytípust a listából (szám szerint):");
                foreach (hazifeladat.DAL1.Models.Enums.PlaceTypes pt in Enum.GetValues(typeof(hazifeladat.DAL1.Models.Enums.PlaceTypes)))
                {
                    Console.WriteLine($"{(int)pt} - {pt}");
                }
                Console.Write("Helytípus száma: ");
                string? typeStr = Console.ReadLine();
                if (!int.TryParse(typeStr, out int typeNum) || !Enum.IsDefined(typeof(hazifeladat.DAL1.Models.Enums.PlaceTypes), typeNum))
                {
                    displayDividerLine(90, '=');
                    Console.WriteLine("Érvénytelen helytípus.");
                    displayDividerLine(90, '=');
                    return;
                }
                placeType = (hazifeladat.DAL1.Models.Enums.PlaceTypes)typeNum;
            }

            displayDividerLine(90, '-');
            Console.Write("Érkezés (ÉÉÉÉ-HH-NN): ");
            string? arrivalStr = Console.ReadLine();
            if (!DateTime.TryParse(arrivalStr, out DateTime arrival))
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Érvénytelen érkezési dátum.");
                displayDividerLine(90, '=');
                return;
            }

            displayDividerLine(90, '-');
            Console.Write("Távozás (ÉÉÉÉ-HH-NN): ");
            string? departureStr = Console.ReadLine();
            if (!DateTime.TryParse(departureStr, out DateTime departure))
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Érvénytelen távozási dátum.");
                displayDividerLine(90, '=');
                return;
            }

            if (arrival.Date >= departure.Date)
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Az érkezésnek korábbinak kell lennie, mint a távozásnak.");
                displayDividerLine(90, '=');
                return;
            }

            displayDividerLine(90, '-');
            Console.Write("Vendégek száma: ");
            string? guestsStr = Console.ReadLine();
            if (!int.TryParse(guestsStr, out int numberOfGuests) || numberOfGuests <= 0)
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Érvénytelen vendégszám.");
                displayDividerLine(90, '=');
                return;
            }

            displayDividerLine(90, '-');
            Console.Write("Vendég neve: ");
            string? guestName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(guestName))
            {
                displayDividerLine(90, '=');
                Console.WriteLine("A vendég neve kötelező.");
                displayDividerLine(90, '=');
                return;
            }

            try
            {
                if (placeId.HasValue)
                {
                    var booking = await bookingService.CreateBookingForPlaceAsync(
                        user.Id,
                        placeId.Value,
                        arrival,
                        departure,
                        numberOfGuests,
                        guestName);

                    displayDividerLine(90, '=');
                    Console.WriteLine($"Foglalás sikeres! Foglalás azonosító: {booking.BookingId}");
                    displayDividerLine(90, '=');
                }
                else if (placeType.HasValue)
                {
                    var booking = await bookingService.CreateBookingForPlaceTypeAsync(
                        user.Id,
                        placeType.Value,
                        arrival,
                        departure,
                        numberOfGuests,
                        guestName);

                    displayDividerLine(90, '=');
                    Console.WriteLine($"Foglalás sikeres! Hely: {booking.PlaceId}, Foglalás azonosító: {booking.BookingId}");
                    displayDividerLine(90, '=');
                }
                else
                {
                    displayDividerLine(90, '=');
                    Console.WriteLine("Hiba: nem választott sem helyet, sem helytípust.");
                    displayDividerLine(90, '=');
                }
            }
            catch (ArgumentException ex)
            {
                displayDividerLine(90, '=');
                Console.WriteLine($"Hibás adatok: {ex.Message}");
                displayDividerLine(90, '=');
            }
            catch (InvalidOperationException ex)
            {
                displayDividerLine(90, '=');
                Console.WriteLine($"Foglalás sikertelen: {ex.Message}");
                displayDividerLine(90, '=');
            }
            catch (Exception ex)
            {
                displayDividerLine(90, '=');
                Console.WriteLine($"Váratlan hiba történt: {ex.Message}");
                displayDividerLine(90, '=');
            }
        }

        private static async Task displayUserBookings(BookingService bookingService, User user)
        {
            var bookings = await bookingService.GetUserBookingsAsync(user.Id);
            if (bookings == null || bookings.Count == 0)
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Nincsenek foglalásai.");
                displayDividerLine(90, '=');
                return;
            }

            displayDynamicDividerLine(bookings[0].ToString(), '=');
            Console.WriteLine("Saját foglalások:");
            displayDynamicDividerLine(bookings[0].ToString(), '-');
            foreach (var b in bookings)
            {
                Console.WriteLine(b.ToString());
            }
            displayDynamicDividerLine(bookings[0].ToString(), '=');
        }

        private static async Task handleModifyBooking(BookingService bookingService, User user)
        {
            displayDividerLine(90, '=');
            Console.Write("Adja meg a módosítani kívánt foglalás azonosítóját: ");
            string? idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out int bookingId))
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Érvénytelen azonosító.");
                displayDividerLine(90, '=');
                return;
            }

            var existing = await bookingService.GetAllBookingsAsync();
            var match = existing.SingleOrDefault(b => b.BookingId == bookingId);
            if (match == null)
            {
                displayDividerLine(90, '=');
                Console.WriteLine("A megadott foglalás nem található.");
                displayDividerLine(90, '=');
                return;
            }
            if (user.Role != hazifeladat.DAL1.Models.Enums.UserRole.ADMIN && match.UserId != user.Id)
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Nincs jogosultsága a módosításhoz.");
                displayDividerLine(90, '=');
                return;
            }

            displayDividerLine(90, '-');
            Console.Write("Új érkezés (ÉÉÉÉ-HH-NN): ");
            string? newArrivalStr = Console.ReadLine();
            if (!DateTime.TryParse(newArrivalStr, out DateTime newArrival))
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Érvénytelen dátum.");
                displayDividerLine(90, '=');
                return;
            }

            displayDividerLine(90, '-');
            Console.Write("Új távozás (ÉÉÉÉ-HH-NN): ");
            string? newDepartureStr = Console.ReadLine();
            if (!DateTime.TryParse(newDepartureStr, out DateTime newDeparture))
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Érvénytelen dátum.");
                displayDividerLine(90, '=');
                return;
            }

            if (newArrival.Date >= newDeparture.Date)
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Az érkezésnek korábbinak kell lennie, mint a távozásnak.");
                displayDividerLine(90, '=');
                return;
            }

            displayDividerLine(90, '-');
            Console.Write("Új vendégszám: ");
            string? newGuestsStr = Console.ReadLine();
            if (!int.TryParse(newGuestsStr, out int newGuests) || newGuests <= 0)
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Érvénytelen vendégszám.");
                displayDividerLine(90, '=');
                return;
            }

            var result = await bookingService.ModifyBookingAsync(bookingId, newArrival, newDeparture, newGuests);
            if (result.Success)
            {
                displayDynamicDividerLine(result.ToString(), '=');
                Console.WriteLine("Foglalás sikeresen módosítva:");
                displayDynamicDividerLine(result.ToString(), '-');
                Console.WriteLine(result.UpdatedBooking?.ToString());
                displayDynamicDividerLine(result.ToString(), '=');
            }
            else
            {
                displayDynamicDividerLine(result.Alternatives.ToString(), '=');
                Console.WriteLine($"Módosítás sikertelen: {result.ErrorMessage}");
                if (result.Alternatives != null && result.Alternatives.Count > 0)
                {
                    displayDynamicDividerLine(result.Alternatives.ToString(), '=');
                    Console.WriteLine("Felajánlott alternatívák:");
                    displayDynamicDividerLine(result.Alternatives.ToString(), '-');
                    foreach (var alt in result.Alternatives)
                    {
                        Console.WriteLine($"HelyId: {alt.PlaceId}, Típus: {alt.PlaceType}, Érkezés: {alt.SuggestedArrival:d}, Távozás: {alt.SuggestedDeparture:d}");
                    }
                }
                displayDynamicDividerLine(result.Alternatives.ToString(), '=');
            }
        }

        private static async Task handleDeleteBooking(BookingService bookingService, User user)
        {
            displayDividerLine(90, '=');
            Console.Write("Adja meg a törölni kívánt foglalás azonosítóját: ");
            string? bookingIdStr = Console.ReadLine();
            if (int.TryParse(bookingIdStr, out int bookingId))
            {
                bool success = await bookingService.CancelBookingAsync(bookingId, user.Id, user.Role);
                if (success)
                {
                    displayDividerLine(90, '=');
                    Console.WriteLine("Foglalás sikeresen törölve.");
                    displayDividerLine(90, '=');
                }
                else
                {
                    displayDividerLine(90, '=');
                    Console.WriteLine("Hiba történt a foglalás törlése során vagy nincs jogosultsága a törlésre.");
                    displayDividerLine(90, '=');
                }
            }
            else
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Érvénytelen foglalás azonosító.");
                displayDividerLine(90, '=');
            }
        }

        private static async Task displayPlaces(BookingService bookingService)
        {
            var from = DateTime.Today;
            var to = DateTime.Today.AddDays(365);
            var availability = await bookingService.GetAvailabilityAsync(from, to);
            int linelen = availability[0].ToString().Length;

            if (availability == null || availability.Count == 0)
            {
                displayDividerLine(linelen, '=');
                Console.WriteLine("Nincsenek helyek a rendszerben.");
                displayDividerLine(linelen, '=');
                return;
            }

            displayDividerLine(linelen, '=');
            Console.WriteLine("Helyek és elérhetőség (összefoglaló):");
            displayDividerLine(linelen, '-');
            foreach (var p in availability)
            {
                Console.WriteLine(p.ToString());
            }
            displayDividerLine(linelen, '=');
        }

        private static async Task handleAdminManageBooking(BookingService bookingService, User admin)
        {
            displayDividerLine(90, '=');
            Console.WriteLine("Admin: foglalás kezelése (módosítás/törlés).");
            Console.Write("Adja meg a foglalás azonosítóját: ");
            string? idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out int bookingId))
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Érvénytelen azonosító.");
                displayDividerLine(90, '=');
                return;
            }

            displayDividerLine(90, '=');
            Console.WriteLine("1 = Módosítás, 2 = Törlés, más = vissza");
            int action = readMenuChoice();
            if (action == 1)
            {
                await handleModifyBooking(bookingService, admin);
            }
            else if (action == 2)
            {
                bool success = await bookingService.CancelBookingAsync(bookingId, admin.Id, admin.Role);
                displayDividerLine(90, '=');
                Console.WriteLine(success ? "Törölve." : "Törlés sikertelen.");
                displayDividerLine(90, '=');
            }
        }

        private static Task handleAddPlaceNotImplemented()
        {
            displayDividerLine(90, '=');
            Console.WriteLine("Hely hozzáadása nincs implementálva.");
            displayDividerLine(90, '=');
            return Task.CompletedTask;
        }

        private static async Task displayAllBookings(BookingService bookingService)
        {
            var allBookings = await bookingService.GetAllBookingsAsync();
            if (allBookings == null || allBookings.Count == 0)
            {
                displayDividerLine(90, '=');
                Console.WriteLine("Nincsenek foglalások.");
                displayDividerLine(90, '=');
                return;
            }
            int linelen = allBookings[0].ToString().Length;
            displayDividerLine(linelen, '=');
            Console.WriteLine("Összes foglalás:");
            displayDividerLine(linelen, '-');
            foreach (var booking in allBookings)
            {
                Console.WriteLine(booking.ToString());
            }
            displayDividerLine(linelen, '=');
        }
    }
}
