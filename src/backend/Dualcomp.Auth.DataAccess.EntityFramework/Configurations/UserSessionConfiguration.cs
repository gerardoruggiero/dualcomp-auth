using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dualcomp.Auth.Domain.Users;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.UserId)
                .IsRequired();

            builder.Property(s => s.AccessToken)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(s => s.RefreshToken)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.ExpiresAt)
                .IsRequired();

            builder.Property(s => s.LastUsedAt);

            builder.Property(s => s.IsActive)
                .IsRequired();

            builder.Property(s => s.UserAgent)
                .HasMaxLength(500);

            builder.Property(s => s.IpAddress)
                .HasMaxLength(45); // IPv6 max length

            builder.HasIndex(s => s.AccessToken)
                .IsUnique();

            builder.HasIndex(s => s.RefreshToken)
                .IsUnique();

            builder.HasIndex(s => s.UserId);

            builder.HasIndex(s => s.ExpiresAt);
        }
    }
}
