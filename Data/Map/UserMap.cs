using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteTecnicoPloomes.Models;

namespace TesteTecnicoPloomes.Data.Map
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            try
            {
                builder.HasKey(user => user.Id);
                builder.Property(user => user.Username).IsRequired().HasMaxLength(16);
                builder.Property(user => user.Password).IsRequired();
                builder.Property(user => user.Role).IsRequired().HasConversion<int>();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
