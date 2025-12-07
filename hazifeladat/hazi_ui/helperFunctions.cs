using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static hazi_ui.menuDisplays;
using static hazifeladat.Logic1.displayConfig;

namespace hazi_ui
{
    internal class helperFunctions
    {
        public static int readMenuChoice()
        {
            Console.Write("Választás: ");
            string? choiceStr = Console.ReadLine();
            if (int.TryParse(choiceStr, out int choice))
            {
                return choice;
            }
            return -1; //érvénytelen választás
        }

        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
        }

        public static void DisplayError(string message)
        {
            displayDividerLine(DefaultDividerWidth, '=');
            Console.WriteLine(message);
            displayDividerLine(DefaultDividerWidth, '=');
        }

        public static void DisplaySuccess(string message)
        {
            displayDividerLine(DefaultDividerWidth, '=');
            Console.WriteLine(message);
            displayDividerLine(DefaultDividerWidth, '=');
        }

        public static bool DisplayInvalidChoice()
        {
            displayDividerLine(ShortDividerWidth, '=');
            Console.WriteLine("Érvénytelen választás, próbálja újra.");
            displayDividerLine(ShortDividerWidth, '=');
            return true;
        }
    }
}
