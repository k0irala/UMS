using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Models.Entities;

namespace UMS.Configurations
{
    public class ManagerRefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenManager>
    {
        public void Configure(EntityTypeBuilder<RefreshTokenManager> builder)
        {
            builder.ToTable(nameof(RefreshTokenManager));
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(rt => rt.RefreshUserToken)
                .IsRequired()
                .HasMaxLength(500);
            #region Foreign Keys
            builder.HasOne(rt => rt.Manager)
                .WithMany(m => m.RefreshTokens)
                .HasForeignKey(rt => rt.ManagerId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion
        }
    }
}
