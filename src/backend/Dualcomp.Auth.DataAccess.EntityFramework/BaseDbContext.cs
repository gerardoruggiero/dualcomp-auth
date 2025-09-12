using Microsoft.EntityFrameworkCore;
using Dualcomp.Auth.DataAccess.EntityFramework.Configurations;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Users;

namespace Dualcomp.Auth.DataAccess.EntityFramework
{
	public class BaseDbContext : DbContext
	{
		public DbSet<Company> Companies => Set<Company>();
		public DbSet<Employee> Employees => Set<Employee>();
		public DbSet<User> Users => Set<User>();
		public DbSet<UserSession> UserSessions => Set<UserSession>();
		public DbSet<AddressTypeEntity> AddressTypes => Set<AddressTypeEntity>();
		public DbSet<EmailTypeEntity> EmailTypes => Set<EmailTypeEntity>();
		public DbSet<PhoneTypeEntity> PhoneTypes => Set<PhoneTypeEntity>();
		public DbSet<SocialMediaTypeEntity> SocialMediaTypes => Set<SocialMediaTypeEntity>();
		public DbSet<CompanyAddress> CompanyAddresses => Set<CompanyAddress>();
		public DbSet<CompanyEmail> CompanyEmails => Set<CompanyEmail>();
		public DbSet<CompanyPhone> CompanyPhones => Set<CompanyPhone>();
		public DbSet<CompanySocialMedia> CompanySocialMedias => Set<CompanySocialMedia>();
		
		public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfiguration(new CompanyConfiguration());
			modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
			modelBuilder.ApplyConfiguration(new UserConfiguration());
			modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
			modelBuilder.ApplyConfiguration(new AddressTypeConfiguration());
			modelBuilder.ApplyConfiguration(new EmailTypeConfiguration());
			modelBuilder.ApplyConfiguration(new PhoneTypeConfiguration());
			modelBuilder.ApplyConfiguration(new SocialMediaTypeConfiguration());
			modelBuilder.ApplyConfiguration(new CompanyAddressConfiguration());
			modelBuilder.ApplyConfiguration(new CompanyEmailConfiguration());
			modelBuilder.ApplyConfiguration(new CompanyPhoneConfiguration());
			modelBuilder.ApplyConfiguration(new CompanySocialMediaConfiguration());
		}
	}
}
