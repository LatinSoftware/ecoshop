using UserService.Entities;

namespace UserService.Models;

public record UserModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email {get; set;} = string.Empty;
    public Role Role { get; set; }
    public Address? Address { get; set; }
}
