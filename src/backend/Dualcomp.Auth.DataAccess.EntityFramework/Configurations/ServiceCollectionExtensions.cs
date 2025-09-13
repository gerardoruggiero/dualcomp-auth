using Dualcomp.Auth.DataAccess.EntityFramework.Repositories;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Users.Repositories;
using DualComp.Infraestructure.Caching;
using DualComp.Infraestructure.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBaseInfraestructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMemoryCache();
			services.AddSingleton<ICacheService, MemoryCacheService>();
			
			var connectionString = configuration.GetConnectionString("DefaultConnection") 
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			
			// Configurar DbContext Factory para evitar problemas de concurrencia
			services.AddDbContextFactory<BaseDbContext>(options =>
				options.UseSqlServer(connectionString)
			);
			
			// Registrar DbContext como Scoped para compatibilidad con UnitOfWork
			services.AddScoped<BaseDbContext>(provider =>
			{
				var factory = provider.GetRequiredService<IDbContextFactory<BaseDbContext>>();
				return factory.CreateDbContext();
			});
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
			services.AddScoped<ICompanyRepository, CompanyRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IUserSessionRepository, UserSessionRepository>();
			services.AddScoped<IAddressTypeRepository, AddressTypeRepository>();
			services.AddScoped<IEmailTypeRepository, EmailTypeRepository>();
			services.AddScoped<IPhoneTypeRepository, PhoneTypeRepository>();
			services.AddScoped<ISocialMediaTypeRepository, SocialMediaTypeRepository>();
			services.AddScoped<IEmailValidationRepository, EmailValidationRepository>();
			services.AddScoped<ICompanySettingsRepository, CompanySettingsRepository>();
			services.AddScoped<IUnitOfWork, EfUnitOfWork>();
			return services;
		}
	}
}
