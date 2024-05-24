﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProjectDomain.Entity;

namespace MyProjectDAL.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(30);
            builder.HasData(new List<Role>()
            {
                new()
                {
                    Id = 1,
                    Name = "User"
                },

                new()
                {
                    Id = 2,
                    Name = "Admin"
                },

                new()
                {
                    Id = 3,
                    Name = "Moderator"
                }
            });
        }
    }
}