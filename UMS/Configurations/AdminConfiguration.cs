using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Models.Entities;

namespace UMS.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.ToTable(nameof(Admin));
            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Email)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(rt => rt.Password)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(rt => rt.UserName)
         .IsRequired()
         .HasMaxLength(500);
            builder.Property(rt => rt.FullName)
         .IsRequired()
         .HasMaxLength(500);

            // Foreign Key Configuration
            builder.HasOne(rt => rt.Roles)
                .WithMany(e => e.Admin)
                .HasForeignKey(rt => rt.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
