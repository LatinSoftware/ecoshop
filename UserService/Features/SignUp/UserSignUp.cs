using System.Security.Cryptography;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Abstractions;
using UserService.Database;
using UserService.Entities;
using UserService.Shared;

namespace UserService.Features.SignUp
{
    public class UserSignUp
    {
        public record Command(string Name, string Email, string Password, string CPassword, Address Address) : ICommand;

        public sealed class Handler(ApplicationContext context) : ICommandHandler<Command>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {

                if (request.Password != request.CPassword)
                    return Result.Fail(UserErrorMessage.PasswordsMismatch);

                var password = Password.Create(request.Password);

                if (password.IsFailed)
                    return Result.Fail(password.Errors);

                if (await context.Users.AnyAsync(u => u.Email == new Email(request.Email), cancellationToken: cancellationToken))
                    return Result.Fail(UserErrorMessage.EmailAlreadyExist);


                var user = User.Create(request.Name, new Email(request.Email), password.Value, request.Address);

                if (user.IsFailed)
                    return Result.Fail(user.Errors);

                await context.Users.AddAsync(user.Value, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }


        }

        public sealed class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(u => u.Name).NotNull().NotEmpty().MaximumLength(100);
                RuleFor(u => u.Email).NotNull().EmailAddress();
                RuleFor(u => u.Password).NotNull().NotEmpty().MaximumLength(50);
                RuleFor(u => u.CPassword).NotNull().NotEmpty();
                RuleFor(u => u.Address).NotNull().SetValidator(new AddressValidator());
            }
        }

        public class AddressValidator : AbstractValidator<Address>
        {
            public AddressValidator()
            {
                RuleFor(a => a.Street).NotNull().NotEmpty().MaximumLength(200);
                RuleFor(a => a.Sector).NotNull().NotEmpty().MaximumLength(150);
                RuleFor(a => a.City).NotNull().NotEmpty().MaximumLength(100);
                RuleFor(a => a.Country).NotNull().NotEmpty().MaximumLength(100);
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("signup", async (Command command, ISender sender) =>
                {
                    var result = await sender.Send(command);

                    return result switch
                    {
                        { IsSuccess: true } => Results.Created(),
                        { IsFailed: true, Errors: var error } when result.HasError(error => error.HasMetadata("code", value => (string)value == UserErrorMessage.EmailAlreadyExist.Code)) => Results.Conflict(error),
                        _ => Results.BadRequest(result.Errors)

                    };
                });
            }
        }
    }
}
