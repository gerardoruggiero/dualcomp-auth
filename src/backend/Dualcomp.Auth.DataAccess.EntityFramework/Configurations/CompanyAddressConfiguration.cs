using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class CompanyAddressConfiguration : IEntityTypeConfiguration<CompanyAddress>
	{
		public void Configure(EntityTypeBuilder<CompanyAddress> builder)
		{
			builder.HasKey(ca => ca.Id);
			builder.Property(ca => ca.CompanyId).IsRequired();
			builder.Property(ca => ca.Address).IsRequired().HasMaxLength(500);
			builder.Property(ca => ca.IsPrimary).IsRequired();

			// Configurar AddressTypeId como foreign key sin navegación
			builder.Property(ca => ca.AddressTypeId).IsRequired();


			builder.HasIndex(ca => new { ca.CompanyId, ca.IsPrimary });
		}
	}
}
