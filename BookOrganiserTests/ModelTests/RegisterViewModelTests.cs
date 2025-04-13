using System.ComponentModel.DataAnnotations;
using BookOrganiser.ViewModels.AccountViewModels;

namespace BookOrganiserTests.ModelTests;

public class RegisterViewModelTests
{
    [Fact]
    public void Name_WhenEmpty_ShouldFailValidation()
    {
        // Arrange
        var model = new RegisterViewModel 
        { 
            Name = string.Empty,
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123"
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Name") && r.ErrorMessage == "A name is required.");
    }
        
    [Theory]
    [InlineData("notanemail")]
    [InlineData("invalid@")]
    [InlineData("@example.com")]
    [InlineData(null)]
    [InlineData("")]
    public void Email_WhenInvalidFormat_ShouldFailValidation(string invalidEmail)
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Name = "Test User",
            Email = invalidEmail,
            Password = "Password123",
            ConfirmPassword = "Password123"
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
    }
        
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("first.last@company.org")]
    public void Email_WhenValidFormat_ShouldPassValidation(string validEmail)
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Name = "Test User",
            Email = validEmail,
            Password = "Password123",
            ConfirmPassword = "Password123",
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Password_WhenEmpty_ShouldFailValidation()
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = string.Empty,
            ConfirmPassword = string.Empty
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Password") && r.ErrorMessage == "A password is required.");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567")]
    public void Password_WhenTooShort_ShouldFailValidation(string shortPassword)
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = shortPassword,
            ConfirmPassword = shortPassword
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Password") && 
                                      r.ErrorMessage.Contains("The password must be between 8 and 40 characters long."));
    }
        
    [Fact]
    public void Password_WhenTooLong_ShouldFailValidation()
    {
        var longPassword = new string('x', 41);
        
        // Arrange
        var model = new RegisterViewModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = longPassword,
            ConfirmPassword = longPassword
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Password") && 
                                      r.ErrorMessage.Contains("The password must be between 8 and 40 characters long."));
    }

    [Fact]
    public void ConfirmPassword_WhenEmpty_ShouldFailValidation()
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = string.Empty
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("ConfirmPassword") && 
                                      r.ErrorMessage == "Confirm your password.");
    }

    [Fact]
    public void Password_WhenNotMatchingConfirmPassword_ShouldFailValidation()
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "DifferentPassword123"
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Password") && 
                                      r.ErrorMessage == "Passwords don't match.");
    }

    [Fact]
    public void RegisterViewModel_WhenAllFieldsValid_ShouldPassValidation()
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123"
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}