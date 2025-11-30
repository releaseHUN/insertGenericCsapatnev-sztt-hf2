using hazifeladat.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.Logic.Interfaces
{
    public interface IPlacesService
    {
        bool updatePrice(int newPrice, Places placeID);

        //? bool updateSeasonalPrice(int newSeasonalPrice, DateTime seasonStart, DateTime SeasonEnd);

        bool AddCaompingPlace();

        Places GetOpenPlaces(DateTime startDate, DateTime endDate);


    }
}
