using Bogus;
using FluentAssertions;
using UserService.Entities;
using UserService.Exceptions;
using UserService.Features.SignUp;
using UserService.Shared;
using FluentValidation.TestHelper;
using UserService.Errors;

namespace UserService.Test.SignUp;

public class SignUpTest : BaseTest
{
    [Fact]
    public async Task SignUp_WithValidData_ShouldRegisterUserSuccessfully()
    {
        // Arrange
        var command = new Faker<UserSignUp.Command>()
        .CustomInstantiator(f => new UserSignUp.Command(
            f.Name.FullName(),
            f.Internet.Email(),
            "very_strong_password.",
            "very_strong_password.",
            new Address(
                f.Address.StreetName(),
                f.Address.SecondaryAddress(),
                f.Address.City(), f.Address.Country()
                )
            )).Generate();

        // Act
        var result = await sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        var user = context.Users.FirstOrDefault();
        user.Should().NotBeNull();
        user!.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task SignUp_WithExistingEmail_ShouldReturnDuplicateEmailError()
    {
        // arrange
        var user = await CreateUser();

        var command = new Faker<UserSignUp.Command>()
    .CustomInstantiator(f => new UserSignUp.Command(
        f.Name.FullName(),
        user.Email.Value,
        "very_strong_password.",
        "very_strong_password.",
        new Address(
            f.Address.StreetName(),
            f.Address.SecondaryAddress(),
            f.Address.City(), f.Address.Country()
            )
        )).Generate();

        // act
        var result = await sender.Send(command);

        // assert
        result.IsFailed.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Select(e => e.Message).Should().Contain(UserErrorMessage.EmailAlreadyExist.Message);
    }

    [Fact]
    public async Task SignUp_WithWeakPassword_ShouldReturnValidationError()
    {
        // arrange
        var user = await CreateUser();

        var command = new Faker<UserSignUp.Command>()
    .CustomInstantiator(f => new UserSignUp.Command(
        f.Name.FullName(),
        f.Internet.Email(),
        "admin",
        "admin",
        new Address(
            f.Address.StreetName(),
            f.Address.SecondaryAddress(),
            f.Address.City(), f.Address.Country()
            )
        )).Generate();

        // act
        var result = await sender.Send(command);

        // assert
        result.IsFailed.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.HasError<DomainError>().Should().BeTrue();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(1);
        result.Errors.Select(e => e.Message)
        .Should()
        .Contain(
            [
                UserErrorMessage.PasswordInvalidLength.Message,
                UserErrorMessage.PasswordInvalidSpecialChar.Message
            ]
            );
    }

    [Fact]
    public async Task SignUp_WithEmptyRequiredFields_ShouldReturnValidationError()
    {
        // Arrange
        var command = new UserSignUp.Command(
            string.Empty,
            string.Empty,
            "",
            "",
            new Address(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
            )
            );

        // Act
        var result = await sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailed.Should().BeTrue();

        var validator = new UserSignUp.CommandValidator();
        var validatorResult = await validator.TestValidateAsync(command);
        validatorResult.ShouldHaveValidationErrorFor(u => u.Name);
        validatorResult.ShouldHaveValidationErrorFor(u => u.Email);
        validatorResult.ShouldHaveValidationErrorFor(u => u.Password);
        validatorResult.ShouldHaveValidationErrorFor(u => u.CPassword);
        validatorResult.ShouldHaveValidationErrorFor(u => u.Address.Street);
        validatorResult.ShouldHaveValidationErrorFor(u => u.Address.Sector);
        validatorResult.ShouldHaveValidationErrorFor(u => u.Address.City);
        validatorResult.ShouldHaveValidationErrorFor(u => u.Address.Country);
        result.Errors.Should().HaveCount(validatorResult.Errors.Count);
    }

    [Fact]
    public async Task SignUp_WithInvalidEmailFormat_ShouldReturnValidationError()
    {
        // Arrange
        var command = new Faker<UserSignUp.Command>()
        .CustomInstantiator(f => new UserSignUp.Command(
            f.Name.FullName(),
            "im_a_very_strong_email",
            "very_strong_password.",
            "very_strong_password.",
            new Address(
                f.Address.StreetName(),
                f.Address.SecondaryAddress(),
                f.Address.City(), f.Address.Country()
                )
            )).Generate();

        // Act
        var result = await sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailed.Should().BeTrue();

        var validator = new UserSignUp.CommandValidator();
        var validatorResult = await validator.TestValidateAsync(command);
        validatorResult.ShouldHaveValidationErrorFor(u => u.Email);
        result.Errors.Should().HaveCount(validatorResult.Errors.Count);
    }

    [Fact]
    public async Task SignUp_WithNonMatchingPasswordConfirmation_ShouldReturnValidationError()
    {
        // Arrange
        var command = new Faker<UserSignUp.Command>()
        .CustomInstantiator(f => new UserSignUp.Command(
            f.Name.FullName(),
            f.Internet.Email(),
            "very_strong_password.",
            "very_strong_password.mismatch",
            new Address(
                f.Address.StreetName(),
                f.Address.SecondaryAddress(),
                f.Address.City(), f.Address.Country()
                )
            )).Generate();

        // Act
        var result = await sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailed.Should().BeTrue();
        result.HasError<ApplicationError>().Should().BeTrue();
        result.Errors.Should().ContainEquivalentOf(UserErrorMessage.PasswordsMismatch);

    }

    private async Task<User> CreateUser()
    {
        var password = Password.Create("very_strong_password.");
        var user = new Faker<User>().CustomInstantiator(f =>
            User.Create(
                 f.Name.FullName(),
            new Email("testemail@gmail.com"),
            password.Value,
            new Address(
                f.Address.StreetName(),
                f.Address.SecondaryAddress(),
                f.Address.City(), f.Address.Country()
                )

            ).Value
        ).Generate();

        context.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}
