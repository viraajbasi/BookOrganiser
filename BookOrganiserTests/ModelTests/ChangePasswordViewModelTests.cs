using System.ComponentModel.DataAnnotations;
using BookOrganiser.ViewModels.AccountViewModels;

namespace BookOrganiserTests.ModelTests;

public class ChangePasswordViewModelTests
{
    [Fact]
    public void CurrentPassword_WhenEmpty_ShouldFailValidation()
    {
        // Arrange
        var model = new ChangePasswordViewModel()
        {
            CurrentPassword = string.Empty,
            NewPassword = "Password123",
            ConfirmNewPassword = "Password123",
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("CurrentPassword") && r.ErrorMessage == "Your current password is required.");
    }
    
    [Fact]
    public void NewPassword_WhenEmpty_ShouldFailValidation()
    {
        // Arrange
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "Password123",
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
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "Password123",
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
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "Password123",
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
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "Password123",
            NewPassword = "NewPassword123",
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
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "Password123",
            NewPassword = "NewPassword123",
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
    public void ChangePasswordViewModel_WhenAllFieldsValid_ShouldPassValidation()
    {
        // Arrange
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "Password123",
            NewPassword = "NewPassword123",
            ConfirmNewPassword = "NewPassword123"
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