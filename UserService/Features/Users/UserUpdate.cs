using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Abstractions;
using UserService.Database;
using UserService.Entities;
using UserService.Extensions;
using UserService.Shared;

namespace UserService.Features.Users
{
    public class UserUpdate
    {
        public record Command : ICommand
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public Role? Role { get; set; }
            public Address? Address { get; set; }
        }

        public sealed class Handler(ApplicationContext context) : ICommandHandler<Command>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = new UserId(request.Id);
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId), cancellationToken: cancellationToken);

                if (user == null)
                    return Result.Fail(UserErrorMessage.UserNotFound(userId));

                user.SetName(request.Name);
                user.SetRole(request.Role);
                user.SetAddress(request.Address);

                context.Update(user);

                await context.SaveChangesAsync(cancellationToken);

                return Result.Ok();

            }
        }

        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.Name).NotEmpty().When(x => x.Name != null);
                RuleFor(x => x.Address).SetValidator(new AddressValidator()!).When(x => x.Address != null);
            }
        }

        public class AddressValidator : AbstractValidator<Address>
        {
            public AddressValidator()
            {
                RuleFor(x => x.Street).NotEmpty().When(x => x.Street != null);
                RuleFor(x => x.Sector).NotEmpty().When(x => x.Sector != null);
                RuleFor(x => x.City).NotEmpty().When(x => x.City != null);
                RuleFor(x => x.Country).NotEmpty().When(x => x.Country != null);
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPatch("users/{id:guid}", async (Guid id, Command command, ISender sender) =>
                {
                    command.Id = id;
                    var result = await sender.Send(command);
                    return Results.Ok(result.ToApiResponse());
                }).RequireAuthorization();
            }
        }
    }
}
