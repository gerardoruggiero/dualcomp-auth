using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentTypeEntity>
	{
		public void Configure(EntityTypeBuilder<DocumentTypeEntity> builder)
		{
			builder.HasKey(dt => dt.Id);
            builder.Property(dt => dt.Id).ValueGeneratedNever();
            builder.Property(dt => dt.Name).IsRequired().HasMaxLength(50);
			builder.Property(dt => dt.Description).HasMaxLength(200);
			builder.Property(dt => dt.IsActive).IsRequired();
			builder.HasIndex(dt => dt.Name).IsUnique();
		}
	}
}

