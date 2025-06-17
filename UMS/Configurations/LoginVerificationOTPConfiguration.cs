using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Models.Entities;

namespace UMS.Configurations
{
    public class LoginVerificationOTPConfiguration : IEntityTypeConfiguration<LoginVerificationOTP>
    {
        public void Configure(EntityTypeBuilder<LoginVerificationOTP> builder)
        {
            builder.ToTable("LoginVerificationOTP");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.OTP).IsRequired().HasMaxLength(6);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
        }
    }
}
