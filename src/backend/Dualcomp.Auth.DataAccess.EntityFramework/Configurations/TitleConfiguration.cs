using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class TitleConfiguration : IEntityTypeConfiguration<TitleEntity>
	{
		public void Configure(EntityTypeBuilder<TitleEntity> builder)
		{
			builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedNever();
            builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
			builder.Property(t => t.Description).HasMaxLength(200);
			builder.Property(t => t.IsActive).IsRequired();
			builder.HasIndex(t => t.Name).IsUnique();
		}
	}
}

