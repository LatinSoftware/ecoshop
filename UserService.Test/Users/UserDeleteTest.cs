using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserService.Entities;
using UserService.Errors;
using UserService.Features.Users;
using UserService.Shared;

namespace UserService.Test.Users
{
    public class UserDeleteTest : BaseTest
    {
        [Fact]
        public async Task UserDelete_WhenIdIsGiven_ShouldDeleteUser()
        {
            //arrange
            var user = await CreateUser();
            var command = new UserDelete.Command(user.Id);
            //act
            var result = await sender.Send(command);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();
            var isDeleted = !await context.Users.AnyAsync(x => x.Id == user.Id);
            isDeleted.Should().BeTrue();
        }
        [Fact]
        public async Task UserDelete_WhenIdDontExist_ShoulReturnNotFound()
        {
            //arrange
            var userId = new UserId(Guid.NewGuid());
            var command = new UserDelete.Command(userId);
            //act
            var result = await sender.Send(command);

            //assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.HasError<ApplicationError>().Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().ContainEquivalentOf(UserErrorMessage.UserNotFound(userId));
        }

        private async Task<User> CreateUser()
        {
            Randomizer.Seed = new Random(10);
            var password = Password.Create("very_strong_password.");
            var user = new Faker<User>().CustomInstantiator(f =>
                User.Create(
                     f.Name.FullName(),
                new Email(f.Internet.Email()),
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
}
