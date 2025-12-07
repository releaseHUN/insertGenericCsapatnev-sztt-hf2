using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
