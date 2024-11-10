﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Entities;

namespace ProductService.Database.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id).HasConversion(id => id.Value, comparer => new CategoryId(comparer));
            builder.Property(x => x.Name).IsRequired().HasMaxLength(500);


            builder.HasMany(x => x.Products).WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
