using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
    public class CompanyModuleConfiguration : IEntityTypeConfiguration<CompanyModule>
    {
        public void Configure(EntityTypeBuilder<CompanyModule> builder)
        {
            builder.ToTable("CompanyModules");

            builder.HasKey(cm => new { cm.CompanyId, cm.ModuleId });

            builder.Property(cm => cm.CreatedAt)
                .IsRequired();

            builder.HasOne<Company>()
                .WithMany(c => c.Modules)
                .HasForeignKey(cm => cm.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ModuloEntity>()
                .WithMany()
                .HasForeignKey(cm => cm.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
