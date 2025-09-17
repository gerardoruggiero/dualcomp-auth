using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
	{
		public void Configure(EntityTypeBuilder<Employee> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();
            builder.Property(e => e.FullName)
				.IsRequired()
				.HasMaxLength(200);
			
			builder.Property(e => e.Phone)
				.HasMaxLength(50);

			builder.Property(e => e.CompanyId)
				.IsRequired();

			builder.Property(e => e.UserId);

			builder.Property(e => e.Position)
				.HasMaxLength(100);

			builder.Property(e => e.HireDate);

            builder.Property(e => e.IsActive)
                .IsRequired();

            // Configurar Email como propiedad directa
            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(320);
            
            // Crear índice en Email
            builder.HasIndex(e => e.Email);

            builder.HasIndex(e => e.CompanyId);

            builder.HasIndex(e => e.UserId);

            builder.HasOne(e => e.User)
            .WithOne() // si User no necesita navegar a Employee
            .HasForeignKey<Employee>(e => e.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        }
	}
}
