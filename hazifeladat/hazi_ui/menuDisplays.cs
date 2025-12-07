using hazifeladat.DAL1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazi_ui
{
    internal class menuDisplays
    {

        public static void displayLoginMenu()
        {
            displayDividerLine(40, '=');
            Console.WriteLine("Válasszon a következő lehetőségek közül:");
            displayDividerLine(40, '=');
            Console.WriteLine("1. Belépés");
            Console.WriteLine("2. Regisztráció");
            Console.WriteLine("0. Kilépés");
            displayDividerLine(40, '-');
        }

        public static void displayMenu(User user)
        {
            if (user.Role == hazifeladat.DAL1.Models.Enums.UserRole.ADMIN)
            {
                displayAdminMenu();
            }
            else if (user.Role == hazifeladat.DAL1.Models.Enums.UserRole.GUEST)
            {
                displayGuestMenu();
            }
        }

        private static void displayGuestMenu()
        {
            displayDividerLine(40, '=');
            Console.WriteLine("Válasszon a következő lehetőségek közül:");
            displayDividerLine(40, '=');
            Console.WriteLine("1. Foglalás indítása");
            Console.WriteLine("2. Foglalásaim megtekintése");
            Console.WriteLine("3. Foglalás módosítása");
            Console.WriteLine("4. Foglalás törlése");
            Console.WriteLine("5. Helyek megtekintése");
            Console.WriteLine("0. Kilépés");
            displayDividerLine(40, '-');
        }

        private static void displayAdminMenu()
        {
            displayDividerLine(40, '=');
            Console.WriteLine("Válasszon a következő lehetőségek közül:");
            displayDividerLine(40, '=');
            Console.WriteLine("1. Összes foglalás megtekintése");
            Console.WriteLine("2. Foglalás kezelése");
            Console.WriteLine("3. Helyek megtekintése");
            Console.WriteLine("4. Hely hozzáadása");
            Console.WriteLine("5. Hely módosítása");
            Console.WriteLine("6. Szezonális szabály létrehozása");
            Console.WriteLine("7. Szezonális szabály módosítása");
            Console.WriteLine("8. Szezonális szabályok kiírása");
            Console.WriteLine("0. Kilépés");
            displayDividerLine(40, '-');
        }

        public static void displayDividerLine(int length, char character)
        {
            Console.WriteLine(new string(character, length));
        }

        public static void displayDynamicDividerLine(string input, char character)
        {
            int length = input.Length;
            displayDividerLine(length, character);
        }
    }
}
