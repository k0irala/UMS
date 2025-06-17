using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Models.Entities;

namespace UMS.Configurations
{
    public class EmployeeRefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEmployee>
    {
        public void Configure(EntityTypeBuilder<RefreshTokenEmployee> builder)
        {
            builder.ToTable(nameof(RefreshTokenEmployee));
            builder.HasKey(rt => rt.Id);
            
            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(rt => rt.RefreshUserToken)
                .IsRequired()
                .HasMaxLength(500);
            
            // Foreign Key Configuration
            builder.HasOne(rt => rt.Employee)
                .WithMany(e => e.RefreshTokens)
                .HasForeignKey(rt => rt.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
