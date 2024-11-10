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


            builder.OwnsOne(u => u.Id, id =>
            {
                id.Property(i => i.Value)
                    .HasColumnName("Id") 
                    .IsRequired();
            });

            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email") 
                    .IsRequired()
                    .HasMaxLength(100);
            });

            builder.OwnsOne(u => u.Password, password =>
            {
                password.Property(p => p.Value)
                    .HasColumnName("PasswordHash") 
                    .IsRequired()
                    .HasMaxLength(200);

                password.Property(p => p.Salt)
                    .HasColumnName("PasswordSalt") 
                    .IsRequired()
                    .HasMaxLength(100);
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
