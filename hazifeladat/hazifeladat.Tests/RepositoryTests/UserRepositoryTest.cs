using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Repositories;

namespace hazifeladat.Tests.RepositoryTests;

[TestClass]
public class UserRepositoryTest
{

    private static string CreateUniqueFileName()
           => $"TestUser_{Guid.NewGuid():N}.json";

    private static string GetFilePathInData(string fileName)
    {
        var basePath = AppContext.BaseDirectory;
        var dataDir = Path.Combine(basePath, "Data");
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

        var repo = new UserRepository(fileName);

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
        var repo = new UserRepository(fileName);
        await repo.LoadAsync();

        var user = new User
        {
            Id = 0,  // 0 -> repo ad majd ID-t
                     // ide nyugodtan beírhatsz további property-ket, ha vannak kötelezõk

        };

        // act
        await repo.AddAsync(user);

        // új repository ugyanazzal a fájllal -> ellenõrizzük, hogy tényleg fájlból olvas
        var repo2 = new PlacesRepository(fileName);
        await repo2.LoadAsync();
        var all = await repo2.GetAllAsync();

        // assert
        Assert.AreEqual(1, all.Count, "Exactly one booking should be stored.");
        Assert.AreEqual(1, all[0].Id, "First booking ID should be 1.");
    }

    [TestMethod]
    public async Task DeleteAsync_DeletesBooking_AndPersistsToFile()
    {
        // arrange
        var fileName = CreateUniqueFileName();
        var repo = new UserRepository(fileName);
        await repo.LoadAsync();

        var places1 = new User { Id = 5 };
        var places2 = new User { Id = 7 };
        var places3 = new User { Id = 8 };

        await repo.AddAsync(places1);
        await repo.AddAsync(places2);
        await repo.AddAsync(places3);


        var allBefore = await repo.GetAllAsync();
        Assert.AreEqual(3, allBefore.Count, "Precondition: there should be two bookings.");

        int idToDelete = allBefore[0].Id;

        // act
        await repo.DeleteAsync(idToDelete);

        var repo2 = new PlacesRepository(fileName);
        await repo2.LoadAsync();
        var allAfter = await repo2.GetAllAsync();

        // assert
        Assert.AreEqual(2, allAfter.Count, "Exactly one booking should remain after delete.");
        Assert.IsFalse(allAfter.Any(b => b.Id == idToDelete),
            "Deleted booking should not be present anymore.");
    }
}
