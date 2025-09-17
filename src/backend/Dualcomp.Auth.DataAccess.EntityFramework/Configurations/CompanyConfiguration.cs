using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class CompanyConfiguration : IEntityTypeConfiguration<Company>
	{
		public void Configure(EntityTypeBuilder<Company> builder)
		{
			builder.HasKey(c => c.Id);
			builder.Property(c=> c.Id).ValueGeneratedNever();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);

			builder.OwnsOne(c => c.TaxId, taxId =>
			{
				taxId.Property(t => t.Value).HasColumnName("TaxId").IsRequired().HasMaxLength(50);
				taxId.HasIndex(t => t.Value).IsUnique();

			});

			builder.HasMany(c => c.Employees).WithOne().HasForeignKey(e => e.CompanyId).OnDelete(DeleteBehavior.Cascade);
			builder.HasMany(c => c.Addresses).WithOne().HasForeignKey(ca => ca.CompanyId).OnDelete(DeleteBehavior.Cascade);
			builder.HasMany(c => c.Emails).WithOne().HasForeignKey(ce => ce.CompanyId).OnDelete(DeleteBehavior.Cascade);
			builder.HasMany(c => c.Phones).WithOne().HasForeignKey(cp => cp.CompanyId).OnDelete(DeleteBehavior.Cascade);
			builder.HasMany(c => c.SocialMedias).WithOne().HasForeignKey(csm => csm.CompanyId).OnDelete(DeleteBehavior.Cascade);
        }
	}
}
