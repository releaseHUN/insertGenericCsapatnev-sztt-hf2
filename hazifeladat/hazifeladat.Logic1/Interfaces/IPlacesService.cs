using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Models.Enums;
using hazifeladat.Logic1.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hazifeladat.Logic.Interfaces
{
    public interface IPlacesService
    {
        Task InitializeAsync();

        Task<IReadOnlyList<Places>> GetAllPlacesAsync();
        Task<Places?> GetPlaceByIdAsync(int id);

        Task<IReadOnlyList<Places>> SearchPlacesAsync(
            PlaceTypes? type = null,
            int? minCapacity = null,
            int? maxCapacity = null,
            IEnumerable<string>? requiredAmenities = null,
            PlaceStatus? status = null);

        Task<Places> CreatePlaceAsync(Places place);
        Task<bool> UpdatePlaceAsync(Places place);
        Task<bool> DeletePlaceAsync(int placeId);

        Task<bool> SetPlaceStatusAsync(int placeId, PlaceStatus status);

        Task<IReadOnlyList<PlaceAvailabilityDto>> GetAvailabilityAsync(
            System.DateTime from,
            System.DateTime to,
            PlaceTypes? typeFilter = null,
            int? minCapacity = null);


    }
}
