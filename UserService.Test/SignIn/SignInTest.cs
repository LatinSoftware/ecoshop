using Bogus;
using FluentAssertions;
using UserService.Entities;
using UserService.Features.SignIn;
using UserService.Shared;

namespace UserService.Test.SignIn
{
    public class SignInTest : BaseTest
    {
        [Fact]
        public async Task SignIn_WhenCredentialsAreValid_ShouldReturnAJwtToken()
        {
            //arrange
            await CreateUser();
            var command = new UserSignIn.Command("testemail@gmail.com", "very_strong_password.");

            //act
            var result = await sender.Send(command);

            //assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task SignIn_WhenInvalidCredentials_ShouldReturnAFailure()
        {
            //arrange
            await CreateUser();
            var command = new UserSignIn.Command("testemail@gmail.com", "very_strong");

            //act
            var result = await sender.Send(command);

            //assert
            result.IsFailed.Should().BeTrue();
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
                    f.Address.City(), 
                    f.Address.Country()
                    )

                ).Value
            ).Generate();

            context.Add(user);
            await context.SaveChangesAsync();
            return user;
        }


    }
}
