using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class AddressTypeConfiguration : IEntityTypeConfiguration<AddressTypeEntity>
	{
		public void Configure(EntityTypeBuilder<AddressTypeEntity> builder)
		{
			builder.HasKey(at => at.Id);
			builder.Property(at => at.Name).IsRequired().HasMaxLength(50);
			builder.Property(at => at.Description).HasMaxLength(200);
			builder.Property(at => at.IsActive).IsRequired();
			builder.HasIndex(at => at.Name).IsUnique();
		}
	}
}
