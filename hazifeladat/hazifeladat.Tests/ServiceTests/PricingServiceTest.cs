// File: PricingServiceTests.cs
using System;
using System.Threading.Tasks;
using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Models.Enums;
using hazifeladat.Logic1.Interfaces;
using hazifeladat.Logic1.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace hazifeladat.Tests
{
    [TestClass]
    public class PricingServiceTests
    {
        private InMemoryPlacesRepository _placesRepo = null!;
        private InMemorySeasonRulesRepository _rulesRepo = null!;
        private IPricingService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            var places = new[]
            {
                new Places
                {
                    
                    Type = (PlaceTypes) 0,
                    Capacity = 4,
                    PricePerNight = 5000,
                    Status = PlaceStatus.AVAILABLE
                }
            };
            _placesRepo = new InMemoryPlacesRepository(places);
            _rulesRepo = new InMemorySeasonRulesRepository();
            _service = new PricingService(_placesRepo, _rulesRepo);
        }


        [TestMethod]
        public async Task CalculatePriceForPlaceTypeAsync_UsesMatchingType()
        {
            var quote = await _service.CalculatePriceForPlaceTypeAsync(
                placeType: (PlaceTypes)0,
                arrival: new DateTime(2025, 5, 1),
                departure: new DateTime(2025, 5, 3)
                );

            Assert.AreEqual(2, quote.Nights);
            Assert.AreEqual(5000, quote.PricePerNight);
        }
    }
}
