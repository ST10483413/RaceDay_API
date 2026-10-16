using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RaceDay_API.Controllers;
using RaceDay_API.Data;
using RaceDay_API.DTOs;
using RaceDay_API.Models;
using Xunit;
using Microsoft.EntityFrameworkCore.InMemory;

namespace RaceDay_API.Tests
{
    public class AuthTests
    {
        private ApplicationDbContext GetContext() =>
            new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        private IConfiguration GetConfig() =>
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Jwt:Key", "ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            }).Build();

        [Fact]
        public async Task Register_WithValidParticipant_ReturnsCreated()
        {
            var ctx = GetContext();
            var controller = new AuthController(ctx, GetConfig());

            var result = await controller.Register(new RegisterDto
            {
                Email = "test@test.com",
                Password = "Password123!",
                FullName = "Test",
                Role = "Participant"
            });

            Assert.IsType<Microsoft.AspNetCore.Mvc.ObjectResult>(result);
            Assert.Single(await ctx.Users.ToListAsync());
            var user = await ctx.Users.FirstAsync();
            Assert.NotEqual("Password123!", user.PasswordHash);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsConflict()
        {
            var ctx = GetContext();
            ctx.Users.Add(new User { Email = "x@x.com", PasswordHash = "h", FullName = "X", Role = "Participant" });
            await ctx.SaveChangesAsync();
            var controller = new AuthController(ctx, GetConfig());

            var result = await controller.Register(new RegisterDto
            {
                Email = "x@x.com",
                Password = "P@ss1234!",
                FullName = "Y",
                Role = "Participant"
            });

            Assert.IsType<Microsoft.AspNetCore.Mvc.ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsUnauthorized()
        {
            var ctx = GetContext();
            ctx.Users.Add(new User
            {
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Correct"),
                FullName = "T",
                Role = "Participant"
            });
            await ctx.SaveChangesAsync();
            var controller = new AuthController(ctx, GetConfig());

            var result = await controller.Login(new LoginDto
            {
                Email = "test@test.com",
                Password = "Wrong"
            });

            Assert.IsType<Microsoft.AspNetCore.Mvc.UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithCorrectCredentials_ReturnsToken()
        {
            var ctx = GetContext();
            ctx.Users.Add(new User
            {
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
                FullName = "T",
                Role = "Participant"
            });
            await ctx.SaveChangesAsync();
            var controller = new AuthController(ctx, GetConfig());

            var result = await controller.Login(new LoginDto
            {
                Email = "test@test.com",
                Password = "Pass123!"
            });

            Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        }
    }
}