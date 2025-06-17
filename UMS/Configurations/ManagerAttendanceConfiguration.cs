using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Models.Entities;

namespace UMS.Configurations
{
    public class ManagerAttendanceConfiguration : IEntityTypeConfiguration<ManagerAttendance>
    {
        public void Configure(EntityTypeBuilder<ManagerAttendance> builder)
        {
            builder.ToTable(nameof(ManagerAttendance));

            builder.HasKey(a => a.Id);

            builder.Property(x => x.Remarks)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.IsPresent);

            #region Foreign Keys
            builder.HasOne(x => x.Manager)
                .WithMany(x => x.ManagerAttendances)
                .HasForeignKey(x => x.ManagerId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

        }
    }
}
