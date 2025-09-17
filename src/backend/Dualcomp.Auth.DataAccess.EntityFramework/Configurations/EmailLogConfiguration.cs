using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
    public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
    {
        public void Configure(EntityTypeBuilder<EmailLog> builder)
        {
            builder.HasKey(el => el.Id);
            builder.Property(el => el.CompanyId)
                .IsRequired();

            builder.Property(el => el.ToEmail)
                .IsRequired()
                .HasMaxLength(320);

            builder.Property(el => el.Subject)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(el => el.EmailType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(el => el.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(el => el.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(el => el.SentAt);

            builder.Property(el => el.CreatedAt)
                .IsRequired();

            // Índices para optimizar consultas
            builder.HasIndex(el => el.CompanyId);
            builder.HasIndex(el => el.ToEmail);
            builder.HasIndex(el => el.Status);
            builder.HasIndex(el => el.EmailType);
            builder.HasIndex(el => el.CreatedAt);

            // Relación con Company
            builder.HasOne<Company>()
                .WithMany()
                .HasForeignKey(el => el.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

