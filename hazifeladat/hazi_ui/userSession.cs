using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Repositories;
using hazifeladat.DAL1.Repositories.Interfaces;
using hazifeladat.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using static hazi_ui.menuDisplays;
using static hazi_ui.helperFunctions;
using static hazifeladat.Logic1.displayConfig;

namespace hazi_ui
{
    internal class userSession
    {
        private static SeasonalRulesRepository? seasonalRulesRepo;

        public static async Task handleUserSession(BookingService bookingService, User user)
        {
            seasonalRulesRepo = new SeasonalRulesRepository("SeasonRules.json");
            await seasonalRulesRepo.LoadAsync();

            bool sessionActive = true;
            while (sessionActive)
            {
                displayMenu(user);
                int choice = readMenuChoice();
                if (user.Role == hazifeladat.DAL1.Models.Enums.UserRole.ADMIN)
                {
                    sessionActive = await handleAdminMenuChoice(choice, bookingService, user);
                }
                else if (user.Role == hazifeladat.DAL1.Models.Enums.UserRole.GUEST)
                {
                    sessionActive = await handleGuestMenuChoice(choice, bookingService, user);
                }
            }
        }

        private static async Task<bool> handleAdminMenuChoice(int choice, BookingService bookingService, User user)
        {
            return choice switch
            {
                1 => await displayAllBookings(bookingService) ?? true,
                2 => await handleAdminManageBooking(bookingService, user) ?? true,
                3 => await displayPlaces(bookingService) ?? true,
                4 => await handleAddPlace() ?? true,
                5 => await handleModifyPlace() ?? true,
                6 => await handleAddSeasonalRule() ?? true,
                7 => await handleModifySeasonalRule() ?? true,
                8 => await displaySeasonalRules() ?? true,
                0 => false,
                _ => DisplayInvalidChoice()
            };
        }

        private static async Task<bool> handleGuestMenuChoice(int choice, BookingService bookingService, User user)
        {
            return choice switch
            {
                1 => await handleCreateBooking(bookingService, user) ?? true,
                2 => await displayUserBookings(bookingService, user) ?? true,
                3 => await handleModifyBooking(bookingService, user) ?? true,
                4 => await handleDeleteBooking(bookingService, user) ?? true,
                5 => await displayPlaces(bookingService) ?? true,
                0 => false,
                _ => DisplayInvalidChoice()
            };
        }

        private static bool TryParseDate(string? input, string prompt, out DateTime result)
        {
            result = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayError("Üres dátum.");
                return false;
            }

            if (!DateTime.TryParse(input, out result))
            {
                DisplayError("Érvénytelen dátum formátum.");
                return false;
            }

            return true;
        }

        private static bool TryParseInt(string? input, string fieldName, out int result, int minValue = int.MinValue)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayError($"Üres {fieldName}.");
                return false;
            }

            if (!int.TryParse(input, out result) || result < minValue)
            {
                DisplayError($"Érvénytelen {fieldName}.");
                return false;
            }

            return true;
        }

        private static bool TryParseFloat(string? input, string fieldName, out float result)
        {
            result = 0f;
            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayError($"Üres {fieldName}.");
                return false;
            }

            if (!float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                DisplayError($"Érvénytelen {fieldName}.");
                return false;
            }

            return true;
        }

        private static bool ValidateDateRange(DateTime from, DateTime to, string context = "")
        {
            if (from.Date >= to.Date)
            {
                DisplayError("Az érkezésnek korábbinak kell lennie, mint a távozásnak." + context);
                return false;
            }

            return true;
        }

        private static void DisplayEnumOptions<T>(string prompt) where T : Enum
        {
            Console.WriteLine(prompt);
            foreach (var value in Enum.GetValues(typeof(T)).Cast<T>())
                Console.WriteLine($"{Convert.ToInt32(value)} - {value}");
        }

        private static bool TryParseEnum<T>(string? input, string fieldName, out T result) where T : struct, Enum
        {
            result = default;
            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayError($"Üres {fieldName}.");
                return false;
            }

            if (!int.TryParse(input, out int enumValue) || !Enum.IsDefined(typeof(T), enumValue))
            {
                DisplayError($"Érvénytelen {fieldName}.");
                return false;
            }

            result = (T)(object)enumValue;
            return true;
        }

        private static async Task<bool?> handleCreateBooking(BookingService bookingService, User user)
        {
            displayDividerLine(DefaultDividerWidth, '=');
            Console.WriteLine("Foglalás indítása — adja meg a szükséges adatokat.");
            Console.Write("Szeretne konkrét helyet választani? (i = igen, t = típus szerint / üres = típus szerint): ");
            string? mode = Console.ReadLine()?.Trim().ToLower();

            int? placeId = null;
            hazifeladat.DAL1.Models.Enums.PlaceTypes? placeType = null;

            if (mode == "i" || mode == "igen")
            {
                displayDividerLine(DefaultDividerWidth, '-');
                Console.Write("Adja meg a hely azonosítóját: ");
                if (!TryParseInt(Console.ReadLine(), "hely azonosító", out int parsedPlaceId, minValue: 1))
                {
                    return null;
                }
                placeId = parsedPlaceId;
            }
            else
            {
                displayDividerLine(DefaultDividerWidth, '-');
                DisplayEnumOptions<hazifeladat.DAL1.Models.Enums.PlaceTypes>("Válasszon helytípust a listából (szám szerint):");
                Console.Write("Helytípus száma: ");
                if (!TryParseEnum(Console.ReadLine(), "helytípus", out hazifeladat.DAL1.Models.Enums.PlaceTypes parsedType))
                {
                    return null;
                }
                placeType = parsedType;
            }

            displayDividerLine(DefaultDividerWidth, '-');
            Console.Write("Érkezés (ÉÉÉÉ-HH-NN): ");
            if (!TryParseDate(Console.ReadLine(), "érkezési dátum", out DateTime arrival))
            {
                return null;
            }

            displayDividerLine(DefaultDividerWidth, '-');
            Console.Write("Távozás (ÉÉÉÉ-HH-NN): ");
            if (!TryParseDate(Console.ReadLine(), "távozási dátum", out DateTime departure))
            {
                return null;
            }

            if (!ValidateDateRange(arrival, departure))
            {
                return null;
            }

            displayDividerLine(DefaultDividerWidth, '-');
            Console.Write("Vendégek száma: ");
            if (!TryParseInt(Console.ReadLine(), "vendégszám", out int numberOfGuests, minValue: 1))
            {
                return null;
            }

            displayDividerLine(DefaultDividerWidth, '-');
            Console.Write("Vendég neve: ");
            string? guestName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(guestName))
            {
                DisplayError("A vendég neve kötelező.");
                return null;
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

                    DisplaySuccess($"Foglalás sikeres! Foglalás azonosító: {booking.BookingId}");
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

                    DisplaySuccess($"Foglalás sikeres! Hely: {booking.PlaceId}, Foglalás azonosító: {booking.BookingId}");
                }
                else
                {
                    DisplayError("Hiba: nem választott sem helyet, sem helytípust.");
                }
            }
            catch (ArgumentException ex)
            {
                DisplayError($"Hibás adatok: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                DisplayError($"Foglalás sikertelen: {ex.Message}");
            }
            catch (Exception ex)
            {
                DisplayError($"Váratlan hiba történt: {ex.Message}");
            }

            return null;
        }

        private static async Task<bool?> displayUserBookings(BookingService bookingService, User user)
        {
            var bookings = await bookingService.GetUserBookingsAsync(user.Id);
            if (bookings == null || bookings.Count == 0)
            {
                DisplayError("Nincsenek foglalásai.");
                return null;
            }

            displayDynamicDividerLine(bookings[0].ToString(), '=');
            Console.WriteLine("Saját foglalások:");
            displayDynamicDividerLine(bookings[0].ToString(), '-');
            foreach (var b in bookings)
            {
                Console.WriteLine(b.ToString());
            }
            displayDynamicDividerLine(bookings[0].ToString(), '=');
            return null;
        }

        private static async Task<bool?> handleModifyBooking(BookingService bookingService, User user)
        {
            displayDividerLine(DefaultDividerWidth, '=');

            Console.Write("Adja meg a módosítani kívánt foglalás azonosítóját: ");
            if (!TryParseInt(Console.ReadLine(), "foglalás azonosító", out int bookingId, minValue: 1))
            {
                return null;
            }

            var existing = await bookingService.GetAllBookingsAsync();
            var match = existing.SingleOrDefault(b => b.BookingId == bookingId);
            if (match == null)
            {
                DisplayError("A megadott foglalás nem található.");
                return null;
            }

            if (user.Role != hazifeladat.DAL1.Models.Enums.UserRole.ADMIN && match.UserId != user.Id)
            {
                DisplayError("Nincs jogosultsága a módosításhoz.");
                return null;
            }

            displayDividerLine(DefaultDividerWidth, '-');
            Console.Write("Új érkezés (ÉÉÉÉ-HH-NN): ");
            if (!TryParseDate(Console.ReadLine(), "érkezési dátum", out DateTime newArrival))
            {
                return null;
            }

            displayDividerLine(DefaultDividerWidth, '-');
            Console.Write("Új távozás (ÉÉÉÉ-HH-NN): ");
            if (!TryParseDate(Console.ReadLine(), "távozási dátum", out DateTime newDeparture))
            {
                return null;
            }

            if (!ValidateDateRange(newArrival, newDeparture))
            {
                return null;
            }

            displayDividerLine(DefaultDividerWidth, '-');
            Console.Write("Új vendégszám: ");
            if (!TryParseInt(Console.ReadLine(), "vendégszám", out int newGuests, minValue: 1))
            {
                return null;
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

            return null;
        }

        private static async Task<bool?> handleDeleteBooking(BookingService bookingService, User user)
        {
            displayDividerLine(DefaultDividerWidth, '=');
            Console.Write("Adja meg a törölni kívánt foglalás azonosítóját: ");
            if (!TryParseInt(Console.ReadLine(), "foglalás azonosító", out int bookingId, minValue: 1))
            {
                DisplayError("Érvénytelen foglalás azonosító.");
                return null;
            }

            bool success = await bookingService.CancelBookingAsync(bookingId, user.Id, user.Role);
            if (success)
            {
                DisplaySuccess("Foglalás sikeresen törölve.");
            }
            else
            {
                DisplayError("Hiba történt a foglalás törlése során vagy nincs jogosultsága a törlésre.");
            }

            return null;
        }

        private static async Task<bool?> displayPlaces(BookingService bookingService)
        {
            await bookingService.InitializeAsync();
            var from = DateTime.Today;
            var to = DateTime.Today.AddDays(AvailabilityDays);
            var availability = await bookingService.GetAvailabilityAsync(from, to);

            if (availability == null || availability.Count == 0)
            {
                displayDividerLine(DefaultDividerWidth, '=');
                Console.WriteLine("Nincsenek helyek a rendszerben.");
                displayDividerLine(DefaultDividerWidth, '=');
                return null;
            }

            int linelen = availability[0].ToString().Length;

            displayDividerLine(linelen, '=');
            Console.WriteLine("Helyek és elérhetőségük:");
            displayDividerLine(linelen, '-');
            foreach (var p in availability)
            {
                Console.WriteLine(p.ToString());
            }
            displayDividerLine(linelen, '=');

            return null;
        }

        private static async Task<bool?> handleAdminManageBooking(BookingService bookingService, User admin)
        {
            displayDividerLine(DefaultDividerWidth, '=');
            Console.WriteLine("Admin: foglalás kezelése (módosítás/törlés).");
            Console.Write("Adja meg a foglalás azonosítóját: ");
            if (!TryParseInt(Console.ReadLine(), "foglalás azonosító", out int bookingId, minValue: 1))
            {
                return null;
            }

            displayDividerLine(DefaultDividerWidth, '=');
            Console.WriteLine("1 = Módosítás, 2 = Törlés, más = vissza");
            int action = readMenuChoice();
            if (action == 1)
            {
                await handleModifyBooking(bookingService, admin);
            }
            else if (action == 2)
            {
                bool success = await bookingService.CancelBookingAsync(bookingId, admin.Id, admin.Role);
                displayDividerLine(DefaultDividerWidth, '=');
                Console.WriteLine(success ? "Törölve." : "Törlés sikertelen.");
                displayDividerLine(DefaultDividerWidth, '=');
            }

            return null;
        }

        private static async Task<bool?> handleAddPlace()
        {
            displayDividerLine(DefaultDividerWidth, '=');
            Console.WriteLine("Új hely hozzáadása");

            DisplayEnumOptions<hazifeladat.DAL1.Models.Enums.PlaceTypes>("Válasszon helytípust:");
            Console.Write("Helytípus: ");
            if (!TryParseEnum(Console.ReadLine(), "helytípus", out hazifeladat.DAL1.Models.Enums.PlaceTypes type))
            {
                return null;
            }

            Console.Write("Kapacitás (üres = nincs korlátozás): ");
            string? capStr = Console.ReadLine();
            int? capacity = null;
            if (!string.IsNullOrWhiteSpace(capStr))
            {
                if (!TryParseInt(capStr, "kapacitás", out int capValue, minValue: 0))
                {
                    return null;
                }
                capacity = capValue;
            }

            Console.Write("Ár / éjszaka (pl. 5000): ");
            if (!TryParseFloat(Console.ReadLine(), "ár", out float price))
            {
                return null;
            }

            Console.Write("Felszereltség (vesszővel elválasztva, pl. 'áram,viz'): ");
            string? amenitiesStr = Console.ReadLine();
            List<string>? amenities = null;
            if (!string.IsNullOrWhiteSpace(amenitiesStr))
            {
                amenities = amenitiesStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
            }

            DisplayEnumOptions<hazifeladat.DAL1.Models.Enums.PlaceStatus>("Állapot:");
            Console.Write("Állapot száma: ");
            if (!TryParseEnum(Console.ReadLine(), "állapot", out hazifeladat.DAL1.Models.Enums.PlaceStatus status))
            {
                return null;
            }

            var repo = new PlacesRepository("Places.json");
            bool loaded = await repo.LoadAsync();
            if (!loaded)
            {
                DisplayError("Nem sikerült betölteni a hely adatokat.");
                return null;
            }

            var place = new Places
            {
                Type = type,
                Capacity = capacity,
                PricePerNight = price,
                Amenities = amenities,
                Status = status
            };

            await repo.AddAsync(place);
            DisplaySuccess($"Hely létrehozva. ID: {place.Id}");

            return null;
        }

        private static async Task<bool?> handleModifyPlace()
        {
            displayDividerLine(DefaultDividerWidth, '=');
            Console.WriteLine("Hely módosítása");
            Console.Write("Adja meg a módosítandó hely azonosítóját: ");
            if (!TryParseInt(Console.ReadLine(), "hely azonosító", out int id, minValue: 1))
            {
                return null;
            }

            var repo = new PlacesRepository("Places.json");
            if (!await repo.LoadAsync())
            {
                DisplayError("Nem sikerült betölteni a hely adatokat.");
                return null;
            }

            var place = await repo.GetByIdAsync(id);
            if (place == null)
            {
                DisplayError("Hely nem található.");
                return null;
            }

            Console.WriteLine($"Jelenlegi típus: {place.Type}");
            DisplayEnumOptions<hazifeladat.DAL1.Models.Enums.PlaceTypes>("Új típus (üres = megtartás):");
            Console.Write("Típus: ");
            string? newTypeStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTypeStr))
            {
                if (!TryParseEnum(newTypeStr, "típus", out hazifeladat.DAL1.Models.Enums.PlaceTypes newType))
                {
                    return null;
                }
                place.Type = newType;
            }

            Console.WriteLine($"Jelenlegi kapacitás: {(place.Capacity.HasValue ? place.Capacity.Value.ToString() : "nincs")}");
            Console.Write("Új kapacitás (üres = megtartás): ");
            string? newCapStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newCapStr))
            {
                if (!TryParseInt(newCapStr, "kapacitás", out int newCap, minValue: 0))
                {
                    return null;
                }
                place.Capacity = newCap;
            }

            Console.WriteLine($"Jelenlegi ár: {place.PricePerNight}");
            Console.Write("Új ár (üres = megtartás): ");
            string? newPriceStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newPriceStr))
            {
                if (!TryParseFloat(newPriceStr, "ár", out float newPrice))
                {
                    return null;
                }
                place.PricePerNight = newPrice;
            }

            Console.WriteLine($"Jelenlegi felszereltség: {(place.Amenities != null ? string.Join(", ", place.Amenities) : "<nincs>")}");
            Console.Write("Új felszereltség (vesszővel elválasztva, üres = megtartás): ");
            string? newAmenities = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newAmenities))
            {
                place.Amenities = newAmenities.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            }

            Console.WriteLine($"Jelenlegi állapot: {place.Status}");
            DisplayEnumOptions<hazifeladat.DAL1.Models.Enums.PlaceStatus>("Új állapot (üres = megtartás):");
            Console.Write("Állapot: ");
            string? newStatusStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newStatusStr))
            {
                if (!TryParseEnum(newStatusStr, "állapot", out hazifeladat.DAL1.Models.Enums.PlaceStatus newStatus))
                {
                    return null;
                }
                place.Status = newStatus;
            }

            await repo.UpdateAsync(place);
            DisplaySuccess("Hely sikeresen frissítve.");

            return null;
        }

        private static async Task<bool?> handleAddSeasonalRule()
        {
            if (seasonalRulesRepo == null)
            {
                DisplayError("Szezonális szabály repo nincs inicializálva.");
                return null;
            }

            displayDividerLine(DefaultDividerWidth, '=');
            Console.WriteLine("Szezonális szabály létrehozása");

            Console.Write("Név (leírás): ");
            string? name = Console.ReadLine();

            Console.Write("Kezdő dátum (ÉÉÉÉ-HH-NN): ");
            if (!TryParseDate(Console.ReadLine(), "kezdő dátum", out DateTime from))
            {
                return null;
            }

            Console.Write("Befejező dátum (ÉÉÉÉ-HH-NN): ");
            if (!TryParseDate(Console.ReadLine(), "befejező dátum", out DateTime to))
            {
                return null;
            }

            if (!ValidateDateRange(from, to))
            {
                return null;
            }

            Console.Write("Szorzó (pl. 1.2): ");
            if (!TryParseFloat(Console.ReadLine(), "szorzó", out float multiplier))
            {
                return null;
            }

            DisplayEnumOptions<hazifeladat.DAL1.Models.Enums.PlaceTypes>("Válassza ki a típust(okat) (vesszővel elválasztva):");
            Console.Write("Típusok (pl. 0,2, üres = minden): ");
            string? typesStr = Console.ReadLine();
            List<hazifeladat.DAL1.Models.Enums.PlaceTypes>? appliesTo = ParsePlaceTypesInput(typesStr);

            var rule = new SeasonalRules
            {
                Id = 0,
                Name = name,
                From = from,
                To = to,
                Multiplier = multiplier,
                AppliesToType = appliesTo
            };

            await seasonalRulesRepo.AddOrUpdateAsync(rule);

            var all = await seasonalRulesRepo.GetAllAsync();
            var saved = all.SingleOrDefault(r => 
                r.Name == rule.Name && 
                r.From == rule.From && 
                r.To == rule.To && 
                Math.Abs(r.Multiplier - rule.Multiplier) < FloatEpsilon);

            DisplaySuccess($"Szezonális szabály létrehozva. ID: {saved?.Id ?? rule.Id}");

            return null;
        }

        private static async Task<bool?> handleModifySeasonalRule()
        {
            if (seasonalRulesRepo == null)
            {
                DisplayError("Szezonális szabály repo nincs inicializálva.");
                return null;
            }

            displayDividerLine(DefaultDividerWidth, '=');
            Console.WriteLine("Szezonális szabály módosítása");
            Console.Write("Adja meg a szabály azonosítóját: ");
            if (!TryParseInt(Console.ReadLine(), "szabály azonosító", out int id, minValue: 1))
            {
                return null;
            }

            var rules = await seasonalRulesRepo.GetAllAsync();
            var rule = rules.SingleOrDefault(r => r.Id == id);
            if (rule == null)
            {
                DisplayError("Szabály nem található.");
                return null;
            }

            Console.WriteLine($"Név: {rule.Name}");
            Console.Write("Új név (üres = megtartás): ");
            string? name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) 
                rule.Name = name;

            Console.WriteLine($"Kezdő dátum: {rule.From:yyyy-MM-dd}");
            Console.Write("Új kezdő dátum (üres = megtartás): ");
            string? fromStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(fromStr))
            {
                if (!TryParseDate(fromStr, "kezdő dátum", out DateTime newFrom))
                {
                    return null;
                }
                rule.From = newFrom;
            }

            Console.WriteLine($"Befejező dátum: {rule.To:yyyy-MM-dd}");
            Console.Write("Új befejező dátum (üres = megtartás): ");
            string? toStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(toStr))
            {
                if (!TryParseDate(toStr, "befejező dátum", out DateTime newTo))
                {
                    return null;
                }
                rule.To = newTo;
            }

            if (!ValidateDateRange(rule.From, rule.To))
            {
                return null;
            }

            Console.WriteLine($"Szorzó: {rule.Multiplier}");
            Console.Write("Új szorzó (üres = megtartás): ");
            string? mulStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(mulStr))
            {
                if (!TryParseFloat(mulStr, "szorzó", out float newMul))
                {
                    return null;
                }
                rule.Multiplier = newMul;
            }

            Console.WriteLine("Jelenlegi típusok: " + (rule.AppliesToType != null ? string.Join(", ", rule.AppliesToType) : "<minden>"));
            DisplayEnumOptions<hazifeladat.DAL1.Models.Enums.PlaceTypes>("Új típusok (vesszővel elválasztva, üres = megtartás):");
            Console.Write("Típusok: ");
            string? typesStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(typesStr))
            {
                rule.AppliesToType = ParsePlaceTypesInput(typesStr);
            }

            await seasonalRulesRepo.AddOrUpdateAsync(rule);
            DisplaySuccess("Szezonális szabály frissítve.");

            return null;
        }

        private static List<hazifeladat.DAL1.Models.Enums.PlaceTypes>? ParsePlaceTypesInput(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<hazifeladat.DAL1.Models.Enums.PlaceTypes>();
            
            foreach (var p in parts)
            {
                if (int.TryParse(p.Trim(), out int n) && 
                    Enum.IsDefined(typeof(hazifeladat.DAL1.Models.Enums.PlaceTypes), n))
                {
                    result.Add((hazifeladat.DAL1.Models.Enums.PlaceTypes)n);
                }
            }

            return result.Count > 0 ? result : null;
        }

        private static async Task<bool?> displaySeasonalRules()
        {
            if (seasonalRulesRepo == null)
            {
                DisplayError("Szezonális szabályok repo nincs inicializálva.");
                return null;
            }

            var rules = (await seasonalRulesRepo.GetAllAsync()).OrderBy(r => r.From).ToList();

            displayDividerLine(DefaultDividerWidth, '=');
            Console.WriteLine("Szezonális szabályok listája:");
            displayDividerLine(DefaultDividerWidth, '-');

            if (rules.Count == 0)
            {
                Console.WriteLine("Nincsenek szezonális szabályok.");
                displayDividerLine(DefaultDividerWidth, '=');
                return null;
            }

            const string headerFormat = "{0,4}  {1,-24}  {2,-10}  {3,-10}  {4,7}  {5}";
            Console.WriteLine(headerFormat, "ID", "Név", "Kezdő", "Befejező", "Szorzó", "Típusok");
            displayDividerLine(DefaultDividerWidth, '-');

            foreach (var r in rules)
            {
                string name = string.IsNullOrWhiteSpace(r.Name) ? "<no name>" : r.Name;
                string from = r.From.ToString("yyyy-MM-dd");
                string to = r.To.ToString("yyyy-MM-dd");
                string mult = r.Multiplier.ToString("0.00", CultureInfo.InvariantCulture);
                string types = r.AppliesToType != null && r.AppliesToType.Any() 
                    ? string.Join(",", r.AppliesToType) 
                    : "<minden>";

                Console.WriteLine(headerFormat, r.Id, Truncate(name, 30), from, to, mult, types);
            }

            displayDividerLine(DefaultDividerWidth, '=');

            return null;
        }

        private static async Task<bool?> displayAllBookings(BookingService bookingService)
        {
            var allBookings = await bookingService.GetAllBookingsAsync();
            if (allBookings == null || allBookings.Count == 0)
            {
                displayDividerLine(DefaultDividerWidth, '=');
                Console.WriteLine("Nincsenek foglalások.");
                displayDividerLine(DefaultDividerWidth, '=');
                return null;
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

            return null;
        }
    }
}
