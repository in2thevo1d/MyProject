using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProjectDomain.Entity;

namespace MyProjectDAL.Configurations
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.RefreshToken).IsRequired();
            builder.Property(x => x.RefreshTokenExpityTime).IsRequired();

            builder.HasData(new List<UserToken>()
            {
                new UserToken()
                {
                    Id = 1,
                    RefreshToken = "t34DTj4fF0xSU45654645gf1Z",
                    RefreshTokenExpityTime = DateTime.UtcNow.AddDays(7),
                    UserId = 1
                }
            });
        }
    }
}
