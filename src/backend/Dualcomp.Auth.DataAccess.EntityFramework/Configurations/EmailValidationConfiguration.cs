using Dualcomp.Auth.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
    public class EmailValidationConfiguration : IEntityTypeConfiguration<EmailValidation>
    {
        public void Configure(EntityTypeBuilder<EmailValidation> builder)
        {
            builder.HasKey(ev => ev.Id);
            builder.Property(ev => ev.UserId)
                .IsRequired();

            builder.Property(ev => ev.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(ev => ev.CreatedAt)
                .IsRequired();

            builder.Property(ev => ev.ExpiresAt)
                .IsRequired();

            builder.Property(ev => ev.IsUsed)
                .IsRequired();

            builder.Property(ev => ev.UsedAt);

            // Índices para optimizar consultas
            builder.HasIndex(ev => ev.UserId);
            builder.HasIndex(ev => ev.Token)
                .IsUnique();
            builder.HasIndex(ev => ev.ExpiresAt);
            builder.HasIndex(ev => ev.IsUsed);

            // Índice compuesto para limpieza de tokens expirados
            builder.HasIndex(ev => new { ev.ExpiresAt, ev.IsUsed })
                .HasFilter("IsUsed = 0");

            // Relación con User
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(ev => ev.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

