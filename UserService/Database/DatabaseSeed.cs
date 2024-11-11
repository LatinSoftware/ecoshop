using System;
using Bogus;
using UserService.Entities;

namespace UserService.Database;

public static class DatabaseSeed
{
    public static async Task SeedAsync(ApplicationContext context)
    {
        context.Database.EnsureCreated();
        if (context.Users.Any())
            return;
        Randomizer.Seed = new Random(500);
        var users = new Faker<User>().CustomInstantiator(f =>
            User.Create(
                 f.Name.FullName(),
            new Email(f.Internet.Email()),
            Password.Create("very_strong_password.").Value,
            new Address(
                f.Address.StreetName(),
                f.Address.SecondaryAddress(),
                f.Address.City(), f.Address.Country()
                )
            ).Value
        ).Generate(50);


        context.AddRange(users);
        await context.SaveChangesAsync();
    }
}
