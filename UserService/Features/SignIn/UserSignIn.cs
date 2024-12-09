

using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Abstractions;
using UserService.Database;
using UserService.Extensions;
using UserService.Providers;
using UserService.Shared;

namespace UserService.Features.SignIn
{
    public class UserSignIn
    {
        public record Command(string Email, string Password): ICommand<string>;
        public sealed class Handler(ApplicationContext context, TokenProvider tokenProvider) : ICommandHandler<Command, string>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email.Equals(new Entities.Email(request.Email)), cancellationToken: cancellationToken);

                if (user == null)
                    return Result.Fail(UserErrorMessage.PasswordsMismatch);

                if(!user.PasswordMatch(request.Password))
                    return Result.Fail(UserErrorMessage.PasswordsMismatch);

                var token = tokenProvider.Create(user);

                return Result.Ok(token);
            }
        }

        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(v => v.Email).NotEmpty().NotNull().EmailAddress();
                RuleFor(v => v.Password).NotEmpty().NotNull().MaximumLength(50);
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("signin", async (Command command, ISender sender) =>
                {
                    var result = await sender.Send(command);


                    return result switch
                    {
                        { IsSuccess: true } => Results.Ok(result.ToApiResponse(StatusCodes.Status200OK)),
                        _ => Results.BadRequest(result.ToApiResponse(errorCode: StatusCodes.Status400BadRequest)),
                    };
                });
            }
        }
    }
}
