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
                    Id = 1,
                    Type = (PlaceTypes) 0,
                    Capacity = 4,
                    PricePerNight = 5000m,
                    Status = PlaceStatus.AVAILABLE
                }
            };
            _placesRepo = new InMemoryPlacesRepository(places);
            _rulesRepo = new InMemorySeasonRulesRepository();
            _service = new PricingService(_placesRepo, _rulesRepo);
        }

        [TestMethod]
        public async Task CalculatePriceAsync_UsesBasePriceAndNights()
        {
            var quote = await _service.CalculatePriceAsync(
                placeId: 1,
                arrival: new DateTime(2025, 6, 1),
                departure: new DateTime(2025, 6, 4));

            Assert.AreEqual(3, quote.Nights);
            Assert.AreEqual(5000m, quote.PricePerNight);
            Assert.AreEqual(15000m, quote.TotalPrice);
        }

        [TestMethod]
        public async Task CalculatePriceAsync_AppliesSeasonMultiplier()
        {
            var rule = new SeasonalRules
            {
                Id = 0,
                From = new DateTime(2025, 7, 1),
                To = new DateTime(2025, 8, 1),
                Multiplier = 1.5m,
                AppliesToType = [(PlaceTypes)0, (PlaceTypes)1],
                Name = "Fõszezon"
            };
            await _rulesRepo.AddOrUpdateAsync(rule);

            var quote = await _service.CalculatePriceAsync(
                placeId: 1,
                arrival: new DateTime(2025, 7, 10),
                departure: new DateTime(2025, 7, 13));

            Assert.AreEqual(3, quote.Nights);
            Assert.AreEqual(5000m * 1.5m, quote.PricePerNight);
            Assert.AreEqual(5000m * 1.5m * 3, quote.TotalPrice);
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
            Assert.AreEqual(5000m, quote.PricePerNight);
        }
    }
}
