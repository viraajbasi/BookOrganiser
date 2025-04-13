
using System.ComponentModel.DataAnnotations;
using BookOrganiser.ViewModels.AccountViewModels;

namespace BookOrganiserTests.ModelTests;

public class VerifyEmailViewModelTests
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
        var model = new VerifyEmailViewModel { Email = invalidEmail };
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
        var model = new VerifyEmailViewModel { Email = validEmail };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}