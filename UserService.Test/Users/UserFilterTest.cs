using System;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Entities;
using UserService.Features.Users;
using UserService.Models;

namespace UserService.Test.Users;

public class UserFilterTest : BaseTest
{
    public UserFilterTest()
    {
        CreateUsers().GetAwaiter().GetResult();
    }
    [Fact]
    public async Task UserFilter_WithoutFilters_ShouldReturnAllUsers()
    {
        // arrange
        var query = new UserFilter.Query();

        //act
        var result = await sender.Send(query);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();


        var userCount = await context.Users.CountAsync();
        result.Value.Metadata.TotalItems.Should().Be(userCount);
    }

    [Fact]
    public async Task UserFilter_ByPartialName_ShouldReturnMatchingUsers()
    {
        // arrange
        var query = new UserFilter.Query
        {
            Name = "test"
        };

        //act
        var result = await sender.Send(query);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        var userCount = await context.Users.CountAsync(x => x.Name.ToLower().Contains(query.Name.ToLower()));
        result.Value.Metadata.TotalItems.Should().Be(userCount);
    }

    [Fact]
    public async Task UserFilter_ByEmail_ShouldReturnMatchingUsers()
    {
        // arrange
        var query = new UserFilter.Query
        {
            Email = "Missouri.Herman@gmail.com"
        };

        //act
        var result = await sender.Send(query);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        var userCount = await context.Users.CountAsync(x => x.Email == new Email(query.Email));
        result.Value.Metadata.TotalItems.Should().Be(userCount);
    }

    [Fact]
    public async Task UserFilter_ByAddress_ShouldReturnMatchingUsers()
    {
        // arrange
        var query = new UserFilter.Query
        {
            City = "Santo Domingo",
            Country = "Dominican Republic"
        };

        //act
        var result = await sender.Send(query);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Data.Should().NotBeEmpty();
        var userCount = await context.Users.CountAsync(x => x.Address.City.Contains(query.City) && x.Address.Country.Contains(query.Country));
        result.Value.Metadata.TotalItems.Should().Be(userCount);
    }

    [Fact]
    public async Task SearchProducts_WithMultipleFiltersNoResults_ShouldReturnEmptyList()
    {
        // arrange
        var query = new UserFilter.Query
        {
            City = "Test",
            Country = "Haiti"
        };

        //act
        var result = await sender.Send(query);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task UserFilter_WithPagination_ShouldReturnCorrectSubset()
    {
        // arrange
        var query = new UserFilter.Query
        {
            Page = 1,
            PageSize = 10
        };

        //act
        var result = await sender.Send(query);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        result.Value.Data.Should().NotBeEmpty();

        result.Value.Data.Should().HaveCount(query.PageSize);

        var paginatedData = await context.Users
        .OrderBy(o => o.CreatedAt)
        .Select(u => new UserModel
        {
            Id = u.Id.Value,
            Name = u.Name,
            Email = u.Email.Value,
            Role = u.Role,
            Address = new Address(u.Address.Street, u.Address.Sector, u.Address.City, u.Address.Country)
        })
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .ToListAsync();

        result.Value.Data.Should().ContainInConsecutiveOrder(paginatedData);
    }



    private async Task CreateUsers()
    {
        Randomizer.Seed = new Random(500);
        var password = Password.Create("very_strong_password.");
        var users = new Faker<User>().CustomInstantiator(f =>
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
        ).Generate(50);

        var users2 = new Faker<User>().CustomInstantiator(f =>
            User.Create(
                 string.Concat("test - ", f.Name.FullName()),
            new Email(f.Internet.Email()),
            password.Value,
            new Address(
                f.Address.StreetName(),
                f.Address.SecondaryAddress(),
                "Santo Domingo", "Dominican Republic"
                )
            ).Value
        ).Generate(5);

        var usersToInsert = users.Concat(users2);

        Console.WriteLine(usersToInsert);

        context.AddRange(usersToInsert);
        await context.SaveChangesAsync();

    }
}
