// File: AuthServiceTests.cs
using System;
using System.Threading.Tasks;
using hazifeladat.DAL1.Models;
using hazifeladat.Logic.Interfaces;
using hazifeladat.Logic.Services;
using hazifeladat.Logic1.Interfaces;
using hazifeladat.Logic1.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace hazifeladat.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private InMemoryUserRepository _userRepo = null!;
        private IAuthService _service = null!;

        [TestInitialize]
        public async Task Setup()
        {
            _userRepo = new InMemoryUserRepository();
            _service = new AuthService(_userRepo);

            await _service.RegisterGuestAsync("tamas", "Pasztor tamas", "jelszo123");
        }

        [TestMethod]
        public async Task AuthenticateAsync_ReturnsUser_WhenPasswordCorrect()
        {
            var user = await _service.AuthenticateAsync("tamas", "jelszo123");
            Assert.IsNotNull(user);
            Assert.AreEqual("tamas", user!.UserName);
        }

        [TestMethod]
        public async Task AuthenticateAsync_ReturnsNull_WhenPasswordWrong()
        {
            var user = await _service.AuthenticateAsync("tamas", "rossz");
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task RegisterGuestAsync_Throws_WhenUserNameAlreadyExists()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await _service.RegisterGuestAsync("tamas", "Pasztor tamas", "ujjelszo");
            });
        }

        [TestMethod]
        public async Task ChangePasswordAsync_ChangesPassword_WhenOldMatches()
        {
            var user = await _service.AuthenticateAsync("tamas", "jelszo123");
            Assert.IsNotNull(user);

            bool changed = await _service.ChangePasswordAsync(user!.Id, "jelszo123", "uj123");
            Assert.IsTrue(changed);

            var again = await _service.AuthenticateAsync("tamas", "uj123");
            Assert.IsNotNull(again);
        }

        [TestMethod]
        public async Task ChangePasswordAsync_Fails_WhenOldWrong()
        {
            var user = await _service.AuthenticateAsync("tamas", "jelszo123");
            Assert.IsNotNull(user);

            bool changed = await _service.ChangePasswordAsync(user!.Id, "rossz", "akarmi");
            Assert.IsFalse(changed);
        }
    }
}
