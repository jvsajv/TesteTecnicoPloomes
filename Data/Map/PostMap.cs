using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteTecnicoPloomes.Models;

namespace TesteTecnicoPloomes.Data.Map
{
    public class PostMap : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            try
            {
                builder.HasKey(post => post.Id);
                builder.Property(post => post.Title).IsRequired().HasMaxLength(100);
                builder.Property(post => post.Body).IsRequired();
                builder.Property(post => post.Public).IsRequired();
                builder.Property(post => post.IdOwner).IsRequired();
                builder.Property(post => post.CreatedAt).IsRequired();
                builder.HasOne(post => post.Owner)
                    .WithMany(user => user.Posts)
                    .HasForeignKey(post => post.IdOwner);
                builder.HasIndex(post => post.IdOwner);

            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
