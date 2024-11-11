using Bogus;
using FluentAssertions;
using FluentValidation.TestHelper;
using UserService.Entities;
using UserService.Errors;
using UserService.Features.Users;
using UserService.Shared;

namespace UserService.Test.Users
{
    public class UpdateUserTest : BaseTest
    {
        [Fact]
        public async Task UpdateUser_WithName_ShouldUpdateNameOnly()
        {
            // arrange
            var user = await CreateUser();

            var command = new UserUpdate.Command
            {
                Id =  user.Id.Value,
                Name = "Ramon",
            };

            //act
            var result = await sender.Send(command);
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            //assert
            var updatedUser = await context.Users.FindAsync(user.Id);
            updatedUser!.Name.Should().Be(command.Name);
        }

        [Fact]
        public async Task UpdateUser_WithRole_ShouldUpdateRoleOnly()
        {
            // arrange
            var user = await CreateUser();

            var command = new UserUpdate.Command
            {
                Id = user.Id.Value,
                Role = Role.Admin
            };

            //act
            var result = await sender.Send(command);
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            //assert
            var updatedUser = await context.Users.FindAsync(user.Id);
            updatedUser!.Role.Should().Be(command.Role);
        }

        [Theory]
        [InlineData("calle los llanos #21", "los rios", "santo domingo", "republica dominicana")]
        [InlineData("calle san francisco", null, "Santiago", "republica dominicana")]
        [InlineData("calle ramon matias mella", null, null, null)]
        [InlineData(null, null, null, null)]
        [InlineData(null, null, "NY", "US")]
        [InlineData(null, null, "Miami", null)]
        public async Task UpdateUser_WithAddress_ShouldUpdateAddressOnly(string street, string sector, string city, string country)
        {
            // arrange
            var user = await CreateUser();

            var command = new UserUpdate.Command
            {
                Id = user.Id.Value,
                Address = new Address(street, sector, city, country)
            };

            //act
            var result = await sender.Send(command);
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            //assert
            var updatedUser = await context.Users.FindAsync(user.Id);

            if (command.Address.Street != null)
                updatedUser!.Address.Street.Should().BeEquivalentTo(command.Address.Street);

            if (command.Address.Sector != null)
                updatedUser!.Address.Sector.Should().BeEquivalentTo(command.Address.Sector);

            if (command.Address.Country != null)
                updatedUser!.Address.Country.Should().BeEquivalentTo(command.Address.Country);

            if (command.Address.City != null)
                updatedUser!.Address.City.Should().BeEquivalentTo(command.Address.City);


            updatedUser!.Address.Should().NotBeNull();
            updatedUser!.Address.Street.Should().NotBeNull();
            updatedUser!.Address.Sector.Should().NotBeNull();
            updatedUser!.Address.City.Should().NotBeNull();
            updatedUser!.Address.Country.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateUser_WithoutId_ShouldReturnFailure()
        {
            // arrange
            var user = await CreateUser();

            var command = new UserUpdate.Command
            {
                Name = "Ramon",
                Role = Role.Admin,
            };

            //act
            var result = await sender.Send(command);


            //assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            var validator = new UserUpdate.Validator();
            var validatorResult = await validator.TestValidateAsync(command);
            validatorResult.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public async Task UpdateUser_WithNonExistingId_ShouldReturnFailure()
        {
            // arrange
            var userId = Guid.NewGuid();
            var command = new UserUpdate.Command
            {
                Id = userId,
                Name = "Ramon",
                Role = Role.Admin,
            };

            //act
            var result = await sender.Send(command);


            //assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.HasError<ApplicationError>().Should().BeTrue();
            result.Errors.Should().ContainEquivalentOf(UserErrorMessage.UserNotFound(new UserId(userId)));
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
