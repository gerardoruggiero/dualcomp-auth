using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class CompanyPhoneConfiguration : IEntityTypeConfiguration<CompanyPhone>
	{
		public void Configure(EntityTypeBuilder<CompanyPhone> builder)
		{
			builder.HasKey(cp => cp.Id);
			builder.Property(cp => cp.Id).ValueGeneratedNever();
			builder.Property(cp => cp.CompanyId).IsRequired();
			builder.Property(cp => cp.Phone).IsRequired().HasMaxLength(50);
			builder.Property(cp => cp.IsPrimary).IsRequired();

			// Configurar PhoneTypeId como foreign key sin navegación
			builder.Property(cp => cp.PhoneTypeId).IsRequired();


			builder.HasIndex(cp => new { cp.CompanyId, cp.IsPrimary });
		}
	}
}
