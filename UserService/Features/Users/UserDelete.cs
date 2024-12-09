using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Abstractions;
using UserService.Database;
using UserService.Entities;
using UserService.Extensions;
using UserService.Shared;

namespace UserService.Features.Users
{
    public class UserDelete
    {
        public record Command(UserId UserId) : ICommand;
        public sealed class Handler(ApplicationContext context) : ICommandHandler<Command>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id.Equals(request.UserId), cancellationToken: cancellationToken);
                if (user == null)
                    return Result.Fail(UserErrorMessage.UserNotFound(request.UserId));

                context.Users.Remove(user);

                await context.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapDelete("users/{id:guid}", async (Guid id, ISender sender) =>
                {
                    var result = await sender.Send(new Command(new UserId(id)));

                    if (result.IsFailed)
                        return Results.NotFound(result.ToApiResponse(errorCode: StatusCodes.Status404NotFound));

                    return Results.NoContent();
                }).RequireAuthorization(UConstants.AdminRole);
            }
        }
    }
}
