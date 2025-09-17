using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class SocialMediaTypeConfiguration : IEntityTypeConfiguration<SocialMediaTypeEntity>
	{
		public void Configure(EntityTypeBuilder<SocialMediaTypeEntity> builder)
		{
			builder.HasKey(smt => smt.Id);
            builder.Property(smt => smt.Name).IsRequired().HasMaxLength(50);
			builder.Property(smt => smt.Description).HasMaxLength(200);
			builder.Property(smt => smt.IsActive).IsRequired();
			builder.HasIndex(smt => smt.Name).IsUnique();
		}
	}
}
