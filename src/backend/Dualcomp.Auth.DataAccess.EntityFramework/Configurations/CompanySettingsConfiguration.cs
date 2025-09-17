using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
    public class CompanySettingsConfiguration : IEntityTypeConfiguration<CompanySettings>
    {
        public void Configure(EntityTypeBuilder<CompanySettings> builder)
        {
            builder.HasKey(cs => cs.Id);
            builder.Property(cs => cs.Id)
            .ValueGeneratedNever();
            builder.Property(cs => cs.CompanyId)
                .IsRequired();

            builder.Property(cs => cs.SmtpServer)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(cs => cs.SmtpPort)
                .IsRequired();

            builder.Property(cs => cs.SmtpUsername)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(cs => cs.SmtpPassword)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(cs => cs.SmtpUseSsl)
                .IsRequired();

            builder.Property(cs => cs.SmtpFromEmail)
                .IsRequired()
                .HasMaxLength(320);

            builder.Property(cs => cs.SmtpFromName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(cs => cs.IsActive)
                .IsRequired();

            builder.Property(cs => cs.CreatedAt)
                .IsRequired();

            builder.Property(cs => cs.UpdatedAt);

            builder.Property(cs => cs.CreatedBy);

            builder.Property(cs => cs.UpdatedBy);

            // Índices para optimizar consultas
            builder.HasIndex(cs => cs.CompanyId)
                .IsUnique();
            builder.HasIndex(cs => cs.IsActive);

            // Relación con Company
            builder.HasOne<Company>()
                .WithMany()
                .HasForeignKey(cs => cs.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relaciones con Users (CreatedBy, UpdatedBy)
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(cs => cs.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(cs => cs.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

