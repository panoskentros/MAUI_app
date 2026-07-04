using FluentValidation;
using FluentValidation.Results;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MAUI_app.Tests.Services;

public class UserServiceTests
{
    private AppDbContext GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var databaseContext = new AppDbContext(options);
        databaseContext.Database.EnsureCreated();
        
        databaseContext.Set<ApplicationUser>().RemoveRange(databaseContext.Set<ApplicationUser>());
        databaseContext.SaveChanges();
        
        return databaseContext;
    }

    [Fact]
    public async Task GetAllDoctorsAsync_ReturnsOnlyUsersWithDoctorRole()
    {
        var context = GetDatabaseContext();
        context.Set<ApplicationUser>().AddRange(new List<ApplicationUser>
        {
            new ApplicationUser { Id = 1, UserName = "doc1", Email = "doc1@test.com", Role = UserRole.Doctor, HashedPassword = "123" },
            new ApplicationUser { Id = 2, UserName = "patient1", Email = "patient1@test.com", Role = UserRole.Patient, HashedPassword = "123" },
            new ApplicationUser { Id = 3, UserName = "doc2", Email = "doc2@test.com", Role = UserRole.Doctor, HashedPassword = "123" }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);

        var result = await service.GetAllDoctorsAsync();

        Assert.Equal(2, result.Count);
        Assert.All(result, u => Assert.Equal(UserRole.Doctor, u.Role));
    }

    [Fact]
    public async Task GetDoctorByIdAsync_ReturnsDoctor_WhenIdExists()
    {
        var context = GetDatabaseContext();
        context.Set<ApplicationUser>().AddRange(new List<ApplicationUser>
        {
            new ApplicationUser { Id = 1, UserName = "doc1", Email = "doc1@test.com", Role = UserRole.Doctor, HashedPassword = "123" },
            new ApplicationUser { Id = 2, UserName = "patient1", Email = "patient1@test.com", Role = UserRole.Patient, HashedPassword = "123" }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);

        var result = await service.GetDoctorByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("doc1", result.UserName);
    }

    [Fact]
    public async Task GetAllPatientsAsync_ReturnsOnlyPatients()
    {
        var context = GetDatabaseContext();
        context.Set<ApplicationUser>().AddRange(new List<ApplicationUser>
        {
            new ApplicationUser { Id = 1, UserName = "doc1", Email = "doc1@test.com", Role = UserRole.Doctor, HashedPassword = "123" },
            new ApplicationUser { Id = 2, UserName = "patient1", Email = "patient1@test.com", Role = UserRole.Patient, HashedPassword = "123" }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);

        var result = await service.GetAllPatientsAsync();

        Assert.Single(result);
        Assert.Equal(UserRole.Patient, result[0].Role);
    }

    [Fact]
    public async Task GetPatientByIdAsync_ReturnsPatient_WhenIdExists()
    {
        var context = GetDatabaseContext();
        context.Set<ApplicationUser>().AddRange(new List<ApplicationUser>
        {
            new ApplicationUser { Id = 1, UserName = "doc1", Email = "doc1@test.com", Role = UserRole.Doctor, HashedPassword = "123" },
            new ApplicationUser { Id = 2, UserName = "patient1", Email = "patient1@test.com", Role = UserRole.Patient, HashedPassword = "123" }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);

        var result = await service.GetPatientByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("patient1", result.UserName);
    }

    [Fact]
    public async Task LoginAsync_ReturnsOkAndSetsCurrentUser_WhenCredentialsAreValid()
    {
        var context = GetDatabaseContext();
        string rawPassword = "SecurePassword123";
        string hashedPassword = PasswordHasher.HashPassword(rawPassword);

        var user = new ApplicationUser 
        { 
            Id = 10, 
            UserName = "testuser", 
            Email = "test@example.com", 
            HashedPassword = hashedPassword, 
            Role = UserRole.Patient 
        };
        context.Set<ApplicationUser>().Add(user);
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);

        bool eventRaised = false;
        service.UserChanged += (sender, args) => eventRaised = true;

        var result = await service.LoginAsync("testuser", rawPassword);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("testuser", result.Data.UserName);
        Assert.True(service.IsLoggedIn);
        Assert.Equal(10, service.CurrentUser?.Id);
        Assert.True(eventRaised);
        mockPreferences.Verify(p => p.Set("UserId", 10, null), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ReturnsFail_WhenUserDoesNotExist()
    {
        var context = GetDatabaseContext();
        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);

        var result = await service.LoginAsync("nonexistent", "password");

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.False(service.IsLoggedIn);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsFail_WhenValidationFails()
    {
        var context = GetDatabaseContext();
        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        
        var validationFailures = new List<ValidationFailure> 
        { 
            new ValidationFailure("Email", "Invalid email format") 
        };
        
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);
        var invalidUser = new ApplicationUser { UserName = "baduser", Email = "invalid" };

        var result = await service.RegisterAsync(invalidUser);

        Assert.False(result.Success);
        Assert.Contains("Invalid email format", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsOkAndHashesPassword_WhenValidationSucceeds()
    {
        var context = GetDatabaseContext();
        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);
        var newUser = new ApplicationUser 
        { 
            Id = 5, 
            UserName = "newuser", 
            Email = "newuser@test.com", 
            HashedPassword = "PlainTextPassword" 
        };

        var result = await service.RegisterAsync(newUser);

        Assert.True(result.Success);
        
        var savedUser = await context.Set<ApplicationUser>().FindAsync(5);
        Assert.NotNull(savedUser);
        Assert.NotEqual("PlainTextPassword", savedUser.HashedPassword);
        Assert.True(PasswordHasher.VerifyPassword("PlainTextPassword", savedUser.HashedPassword));
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesDatabaseAndCurrentUserState_WhenValidationSucceeds()
    {
        var context = GetDatabaseContext();
        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        string rawPassword = "123";
        string hashedPassword = PasswordHasher.HashPassword(rawPassword);

        var initialUser = new ApplicationUser { Id = 1, UserName = "oldName", Email = "user@test.com", Role = UserRole.Patient, HashedPassword = hashedPassword };
        context.Set<ApplicationUser>().Add(initialUser);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);
        await service.LoginAsync("oldName", rawPassword); 

        var updatedUser = new ApplicationUser { Id = 1, UserName = "newName", Email = "user@test.com", Role = UserRole.Patient, HashedPassword = hashedPassword };
        
        var result = await service.UpdateUserAsync(updatedUser);

        Assert.True(result.Success);
        Assert.Equal("newName", service.CurrentUser?.UserName);

        var databaseUser = await context.Set<ApplicationUser>().AsNoTracking().FirstOrDefaultAsync(u => u.Id == 1);
        Assert.Equal("newName", databaseUser?.UserName);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFail_WhenValidationFails()
    {
        var context = GetDatabaseContext();
        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        
        var validationFailures = new List<ValidationFailure> 
        { 
            new ValidationFailure("UserName", "Username is required.") 
        };
        
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);
        var invalidUser = new ApplicationUser { Id = 1, UserName = "", Email = "user@test.com" };

        var result = await service.UpdateUserAsync(invalidUser);

        Assert.False(result.Success);
        Assert.Contains("Username is required.", result.Message);
    }

    [Fact]
    public void Logout_ClearsCurrentUserAndTriggersEvent()
    {
        var context = GetDatabaseContext();
        var mockValidator = new Mock<IValidator<ApplicationUser>>();
        var mockPreferences = new Mock<IPreferences>();
        var service = new UserService(context, mockValidator.Object, mockPreferences.Object);

        bool eventRaised = false;
        service.UserChanged += (sender, args) => eventRaised = true;

        service.Logout();

        Assert.Null(service.CurrentUser);
        Assert.False(service.IsLoggedIn);
        Assert.True(eventRaised);
        mockPreferences.Verify(p => p.Remove("UserId", null), Times.Once);
    }
}