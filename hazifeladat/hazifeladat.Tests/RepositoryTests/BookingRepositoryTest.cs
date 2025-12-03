using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Interfaces;
using hazifeladat.DAL1.Repositories.Repositories;


namespace hazifeladat.Tests.RepositoryTests
{
    [TestClass]
    public sealed class BookingRepositoryTest
    {

        private static string CreateUniqueFileName()
            => $"TestBooking_{Guid.NewGuid():N}.json";

    private static string GetFilePathInData(string fileName)
    {
        var basePath = AppContext.BaseDirectory;
        var dataDir = Path.Combine(basePath, "Test");
        Directory.CreateDirectory(dataDir);
        return Path.Combine(dataDir, fileName);
    }

        [TestMethod]
        public async Task LoadAsync_FileDoesNotExist_ReturnsEmptyList()
        {
            // arrange
            var fileName = CreateUniqueFileName();
            var fullPath = GetFilePathInData(fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            var repo = new BookingRepository(fileName);

            // act
            var result = await repo.LoadAsync();
            var all = await repo.GetAllAsync();

            // assert
            Assert.IsTrue(result, "LoadAsync should return true even if file does not exist.");
            Assert.AreEqual(0, all.Count, "New repository should have empty booking list.");
        }

        [TestMethod]
        public async Task AddAsync_AddsBooking_AndPersistsToFile()
        {
            // arrange
            var fileName = CreateUniqueFileName();
            var repo = new BookingRepository(fileName);
            await repo.LoadAsync();

            var booking = new Booking
            {
                BookingId = 0,  // 0 -> repo ad majd ID-t
                // ide nyugodtan beírhatsz további property-ket, ha vannak kötelezők
                PlaceId = "1",
                GuestName = "Tamas",
                NumberOfGuests = 1,
                


            };

            // act
            await repo.AddAsync(booking);

            // új repository ugyanazzal a fájllal -> ellenőrizzük, hogy tényleg fájlból olvas
            var repo2 = new BookingRepository(fileName);
            await repo2.LoadAsync();
            var all = await repo2.GetAllAsync();

            // assert
            Assert.AreEqual(1, all.Count, "Exactly one booking should be stored.");
            Assert.AreEqual(1, all[0].BookingId, "First booking ID should be 1.");
        }

        [TestMethod]
        public async Task DeleteAsync_DeletesBooking_AndPersistsToFile()
        {
            // arrange
            var fileName = CreateUniqueFileName();
            var repo = new BookingRepository(fileName);
            await repo.LoadAsync();

            var booking1 = new Booking { BookingId = 1 };
            var booking2 = new Booking { BookingId = 0 };

            await repo.AddAsync(booking1);
            await repo.AddAsync(booking2);

            var allBefore = await repo.GetAllAsync();
            Assert.AreEqual(2, allBefore.Count, "Precondition: there should be two bookings.");

            int idToDelete = allBefore[0].BookingId;

            // act
            await repo.DeleteAsync(idToDelete);

            var repo2 = new BookingRepository(fileName);
            await repo2.LoadAsync();
            var allAfter = await repo2.GetAllAsync();

            // assert
            Assert.AreEqual(1, allAfter.Count, "Exactly one booking should remain after delete.");
            Assert.IsFalse(allAfter.Any(b => b.BookingId == idToDelete),
                "Deleted booking should not be present anymore.");
        }
    }
}
