using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class ModuloConfiguration : IEntityTypeConfiguration<ModuloEntity>
	{
		public void Configure(EntityTypeBuilder<ModuloEntity> builder)
		{
			builder.ToTable("Modulos");
			
			builder.HasKey(m => m.Id);
			
			builder.Property(m => m.Id).ValueGeneratedNever();
			
			builder.Property(m => m.Name)
				.IsRequired()
				.HasMaxLength(50);
				
			builder.Property(m => m.Description)
				.HasMaxLength(200);
				
			builder.Property(m => m.IsActive)
				.IsRequired();
				
			builder.HasIndex(m => m.Name)
				.IsUnique();
		}
	}
}
