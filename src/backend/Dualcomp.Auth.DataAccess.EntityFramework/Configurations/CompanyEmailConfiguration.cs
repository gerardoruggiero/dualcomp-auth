using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class CompanyEmailConfiguration : IEntityTypeConfiguration<CompanyEmail>
	{
		public void Configure(EntityTypeBuilder<CompanyEmail> builder)
		{
			builder.HasKey(ce => ce.Id);
			builder.Property(ce => ce.CompanyId).IsRequired();
			builder.Property(ce => ce.IsPrimary).IsRequired();

			// Configurar EmailTypeId como foreign key sin navegación
			builder.Property(ce => ce.EmailTypeId).IsRequired();

			// Configurar el value object Email
			builder.OwnsOne(ce => ce.Email, emailBuilder =>
			{
				emailBuilder.Property(e => e.Value)
					.HasColumnName("Email")
					.IsRequired()
					.HasMaxLength(320);
			});


			builder.HasIndex(ce => new { ce.CompanyId, ce.IsPrimary });
		}
	}
}
