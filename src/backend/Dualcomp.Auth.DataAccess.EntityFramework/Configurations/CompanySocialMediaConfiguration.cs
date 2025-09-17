using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class CompanySocialMediaConfiguration : IEntityTypeConfiguration<CompanySocialMedia>
	{
		public void Configure(EntityTypeBuilder<CompanySocialMedia> builder)
		{
			builder.HasKey(csm => csm.Id);
			builder.Property(csm => csm.Id).ValueGeneratedNever();
			builder.Property(csm => csm.CompanyId).IsRequired();
			builder.Property(csm => csm.Url).IsRequired().HasMaxLength(500);
			builder.Property(csm => csm.IsPrimary).IsRequired();

			// Configurar SocialMediaTypeId como foreign key sin navegación
			builder.Property(csm => csm.SocialMediaTypeId).IsRequired();


			builder.HasIndex(csm => new { csm.CompanyId, csm.IsPrimary });
		}
	}
}
