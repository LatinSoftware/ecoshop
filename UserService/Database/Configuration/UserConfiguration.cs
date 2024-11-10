using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Entities;

namespace UserService.Database.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Id).HasConversion(u => u.Value, value => new UserId(value));

            builder.Property(u => u.Name)
           .IsRequired()
           .HasMaxLength(100);

            builder.Property(u => u.CreatedAt)
            .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .IsRequired();

            builder.Property(u => u.Role)
           .HasConversion<string>()
           .IsRequired();



            builder.Property(u => u.Email).HasConversion(u => u.Value, value => new Email(value));

            builder.OwnsOne(u => u.Password, password =>
            {
                password.Property(p => p.Value).HasColumnName("PasswordHash").IsRequired().HasMaxLength(200);
                password.Property(p => p.Salt).HasColumnName("PasswordSalt").IsRequired().HasMaxLength(100);
            });

            builder.OwnsOne(u => u.Address, address =>
            {
                
                address.Property(a => a.Street)
                    .HasColumnName("Street")
                    .IsRequired()
                    .HasMaxLength(200);

                address.Property(a => a.Sector)
                    .HasColumnName("Sector")
                    .IsRequired()
                    .HasMaxLength(150);

                address.Property(a => a.City)
                    .HasColumnName("City")
                    .IsRequired()
                    .HasMaxLength(100);

                address.Property(a => a.Country)
                    .HasColumnName("Country")
                    .IsRequired()
                    .HasMaxLength(100);
            });
        }
    }
}
