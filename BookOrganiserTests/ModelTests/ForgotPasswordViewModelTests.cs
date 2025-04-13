using System.ComponentModel.DataAnnotations;
using BookOrganiser.ViewModels.AccountViewModels;

namespace BookOrganiserTests.ModelTests;

public class ForgotPasswordViewModelTests
{
    [Theory]
    [InlineData("notanemail")]
    [InlineData("invalid@")]
    [InlineData("@example.com")]
    [InlineData("")]
    [InlineData(null)]
    public void Email_WhenInvalidFormat_ShouldFailValidation(string invalidEmail)
    {
        // Arrange
        var model = new ForgotPasswordViewModel
        {
            Email = invalidEmail,
            NewPassword = "Password123",
            ConfirmNewPassword = "Password123"
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
        var model = new ForgotPasswordViewModel
        {
            Email = validEmail,
            NewPassword = "Password123",
            ConfirmNewPassword = "Password123"
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
    public void NewPassword_WhenEmpty_ShouldFailValidation()
    {
        // Arrange
        var model = new ForgotPasswordViewModel
        {
            Email = "test@example.com",
            NewPassword = string.Empty,
            ConfirmNewPassword = string.Empty
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("NewPassword") && r.ErrorMessage == "A password is required.");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567")]
    public void NewPassword_WhenTooShort_ShouldFailValidation(string shortPassword)
    {
        // Arrange
        var model = new ForgotPasswordViewModel
        {
            Email = "test@example.com",
            NewPassword = shortPassword,
            ConfirmNewPassword = shortPassword
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("NewPassword") && 
                                      r.ErrorMessage.Contains("The password must be between 8 and 40 characters long."));
    }

    [Fact]
    public void NewPassword_WhenTooLong_ShouldFailValidation()
    {
        var longPassword = new string('x', 41);
        
        // Arrange
        var model = new ForgotPasswordViewModel
        {
            Email = "test@example.com",
            NewPassword = longPassword,
            ConfirmNewPassword = longPassword
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("NewPassword") && 
                                      r.ErrorMessage.Contains("The password must be between 8 and 40 characters long."));
    }

    [Fact]
    public void ConfirmNewPassword_WhenEmpty_ShouldFailValidation()
    {
        // Arrange
        var model = new ForgotPasswordViewModel
        {
            Email = "test@example.com",
            NewPassword = "Password123",
            ConfirmNewPassword = string.Empty
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("ConfirmNewPassword") && 
                                      r.ErrorMessage == "Confirm your password.");
    }

    [Fact]
    public void NewPassword_WhenNotMatchingConfirmNewPassword_ShouldFailValidation()
    {
        // Arrange
        var model = new ForgotPasswordViewModel
        {
            Email = "test@example.com",
            NewPassword = "Password123",
            ConfirmNewPassword = "DifferentPassword123"
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("NewPassword") && 
                                      r.ErrorMessage == "Passwords don't match.");
    }
    
    [Fact]
    public void ForgotPasswordViewModel_WhenAllFieldsValid_ShouldPassValidation()
    {
        // Arrange
        var model = new ForgotPasswordViewModel
        {
            Email = "test@example.com",
            NewPassword = "Password123",
            ConfirmNewPassword = "Password123"
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