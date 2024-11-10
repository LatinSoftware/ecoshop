using FluentResults;
using FluentValidation;
using UserService.Abstractions;
using UserService.Entities;

namespace UserService.Features.SignUp
{
    public class UserSignUp
    {
        public record Command(string Name, Email Email, string Password, string CPassword, Address Address) : ICommand;

        public sealed class Handler : ICommandHandler<Command>
        {
            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        public sealed class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator() { 
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                throw new NotImplementedException();
            }
        }
    }
}
