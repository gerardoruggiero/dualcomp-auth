using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class PhoneTypeConfiguration : IEntityTypeConfiguration<PhoneTypeEntity>
	{
		public void Configure(EntityTypeBuilder<PhoneTypeEntity> builder)
		{
			builder.HasKey(pt => pt.Id);
            builder.Property(pt => pt.Name).IsRequired().HasMaxLength(50);
			builder.Property(pt => pt.Description).HasMaxLength(200);
			builder.Property(pt => pt.IsActive).IsRequired();
			builder.HasIndex(pt => pt.Name).IsUnique();
		}
	}
}
