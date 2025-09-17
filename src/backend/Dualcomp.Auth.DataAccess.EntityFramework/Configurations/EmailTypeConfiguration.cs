using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class EmailTypeConfiguration : IEntityTypeConfiguration<EmailTypeEntity>
	{
		public void Configure(EntityTypeBuilder<EmailTypeEntity> builder)
		{
			builder.HasKey(et => et.Id);
            builder.Property(et => et.Name).IsRequired().HasMaxLength(50);
			builder.Property(et => et.Description).HasMaxLength(200);
			builder.Property(et => et.IsActive).IsRequired();
			builder.HasIndex(et => et.Name).IsUnique();
		}
	}
}
