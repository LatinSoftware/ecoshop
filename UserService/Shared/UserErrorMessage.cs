using UserService.Exceptions;

namespace UserService.Shared
{
    public static class UserErrorMessage
    {
        public static DomainError NameRequired { get; } = new("User.NameRequired", "Name is required");
        public static DomainError EmailInvalid { get; } = new("User.InvalidEmail", "Email format is invalid");
        public static DomainError PasswordInvalidLenght { get; } = new("User.InvalidPasswordLenght", "Password must have at least 8 character");
        public static DomainError PasswordInvalidSpecialChar { get; } = new("User.InvalidPasswordChar", "Password must at least one special character");
        public static DomainError SteetAddressRequired { get; } = new("User.Address.Steet", "Street is required");
        public static DomainError SectorAddressRequired { get; } = new("User.Address.Sector", "Sector is required");
        public static DomainError CityAddressRequired { get; } = new("User.Address.City", "City is required");
        public static DomainError CountryAddressRequired { get; } = new("User.Address.Country", "Country is required");
    } 
}
