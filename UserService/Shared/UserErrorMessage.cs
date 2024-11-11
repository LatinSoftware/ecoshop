using UserService.Entities;
using UserService.Errors;
using UserService.Exceptions;

namespace UserService.Shared
{
    public static class UserErrorMessage
    {
        public static DomainError NameRequired { get; } = new("User.NameRequired", "Name is required");
        public static DomainError EmailInvalid { get; } = new("User.InvalidEmail", "Email format is invalid");
        public static DomainError PasswordInvalidLength { get; } = new("User.InvalidPasswordLength", "Password must have at least 8 character");
        public static DomainError PasswordInvalidSpecialChar { get; } = new("User.InvalidPasswordChar", "Password must at least one special character");
        public static DomainError StreetAddressRequired { get; } = new("User.Address.Street", "Street is required");
        public static DomainError SectorAddressRequired { get; } = new("User.Address.Sector", "Sector is required");
        public static DomainError CityAddressRequired { get; } = new("User.Address.City", "City is required");
        public static DomainError CountryAddressRequired { get; } = new("User.Address.Country", "Country is required");
        public static ApplicationError EmailAlreadyExist {get; } = new ("Email.AlreadyExist", "Email already exist in our records");
        public static ApplicationError PasswordsMismatch {get; } = new ("Password.Mismatch", "Password mismatch, please repeat again");
        public static ApplicationError UserNotFound(UserId id) => new("User.NotFound", $"User with id '{id.Value}' not found or is deleted");

    }
}
