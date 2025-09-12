using Dualcomp.Auth.DataAccess.EntityFramework.Repositories;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Users;
using DualComp.Infraestructure.Caching;
using DualComp.Infraestructure.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Configurations
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBaseInfraestructure(this IServiceCollection services)
		{
			services.AddMemoryCache();
			services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddDbContext<BaseDbContext>(options =>
				options.UseSqlServer("Server=localhost,14333;Database=DualCompAuth;User Id=sa;Password=Ger_555qqq;TrustServerCertificate=True")
			);
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
			services.AddScoped<ICompanyRepository, CompanyRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IUserSessionRepository, UserSessionRepository>();
			services.AddScoped<IAddressTypeRepository, AddressTypeRepository>();
			services.AddScoped<IEmailTypeRepository, EmailTypeRepository>();
			services.AddScoped<IPhoneTypeRepository, PhoneTypeRepository>();
			services.AddScoped<ISocialMediaTypeRepository, SocialMediaTypeRepository>();
			services.AddScoped<IUnitOfWork, EfUnitOfWork>();
			return services;
		}
	}
}
