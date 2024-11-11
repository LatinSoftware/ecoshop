using FluentResults;
using UserService.Helpers;
using UserService.Shared;

namespace UserService.Entities
{
    public class User
    {
        public UserId Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Email Email { get; private set; }
        public Password Password { get; private set; }
        public Address Address { get; private set; }
        public Role Role { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private User(){}

        private User(UserId id, string name, Email email, Password password, Address address, Role? role)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Address = address;
            Role = role ?? Role.User;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetName(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            Name = name;
        }

        public void SetRole(Role? role)
        {
            if (!role.HasValue)
                return;

            if(Role == role.Value) return;

            Role = role.Value;
        }

        public void SetAddress(Address? address)
        {
            if(address == null) return;
            var street = address.Street ?? Address.Street;
            var sector = address.Sector ?? Address.Sector;
            var country = address.Country ?? Address.Country;
            var city = address.City ?? Address.City;
            Address = new Address(street, sector, city, country);
        }

        public static Result<User> Create(string name, Email email, Password password, Address address, Role? role = Role.User)
        {
            var validationResult = Result.Merge(
                    IsValidName(name),
                    IsValidEmail(email.Value),
                    IsValidAddress(address)
                );

            if (validationResult.IsFailed)
                return validationResult;



            var userId = new UserId(Guid.NewGuid());

            return Result.Ok(new User(userId, name, email, password, address, role));

        }
        private static Result IsValidEmail(string email)
        {

            return Result.FailIf(!email.Contains("@") && !email.Contains("."), UserErrorMessage.EmailInvalid);
        }
        private static Result IsValidAddress(Address address)
        {
            return Result.Merge(
                Result.FailIf(string.IsNullOrWhiteSpace(address.Street), UserErrorMessage.StreetAddressRequired),
                Result.FailIf(string.IsNullOrWhiteSpace(address.Sector), UserErrorMessage.SectorAddressRequired),
                Result.FailIf(string.IsNullOrWhiteSpace(address.City), UserErrorMessage.CityAddressRequired),
                Result.FailIf(string.IsNullOrWhiteSpace(address.Country), UserErrorMessage.CountryAddressRequired)
              );
        }
        private static Result IsValidName(string name)
        {
            return Result.FailIf(string.IsNullOrWhiteSpace(name), UserErrorMessage.NameRequired);
        }

    }

    public enum Role
    {
        Admin,
        User
    }
    public record UserId(Guid Value);
    public record Password()
    {
        private Password(string value, string hash) : this()
        {
            Value = value;
            Salt = hash;
        }
        public string Value { get; private set; } = string.Empty;
        public string Salt { get; private set; } = string.Empty;

        public static Result<Password> Create(string value)
        {

            var validation = Result.Merge(
                    Result.FailIf(value.Length < 8, UserErrorMessage.PasswordInvalidLength),
                    Result.FailIf(!value.Any(char.IsPunctuation), UserErrorMessage.PasswordInvalidSpecialChar)
                );

            if (validation.IsFailed)
                return validation;

            var password = PasswordHasher.HashPassword(value, out string salt);
            return Result.Ok(new Password(password, salt));
        }
    };
    public record Email(string Value);
    public record Address(string Street, string Sector, string City, string Country);
}
