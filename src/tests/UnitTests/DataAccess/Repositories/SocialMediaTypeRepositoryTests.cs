using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.DataAccess.EntityFramework.Repositories;
using Dualcomp.Auth.Domain.Companies;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.UnitTests.DataAccess.Repositories;

public class SocialMediaTypeRepositoryTests : IDisposable
{
	private readonly BaseDbContext _context;
	private readonly SocialMediaTypeRepository _repository;

	public SocialMediaTypeRepositoryTests()
	{
		var options = new DbContextOptionsBuilder<BaseDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_context = new BaseDbContext(options);
		_repository = new SocialMediaTypeRepository(_context);
	}

	[Fact]
	public async Task GetAllAsync_ShouldReturnAllSocialMediaTypes()
	{
		// Arrange
		var socialMediaTypes = new List<SocialMediaTypeEntity>
		{
			SocialMediaTypeEntity.Create("Facebook", "Página de Facebook"),
			SocialMediaTypeEntity.Create("Instagram", "Perfil de Instagram"),
			SocialMediaTypeEntity.Create("LinkedIn", "Página de LinkedIn")
		};

		foreach (var socialMediaType in socialMediaTypes)
		{
			_context.SocialMediaTypes.Add(socialMediaType);
		}
		await _context.SaveChangesAsync();

		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		Assert.Equal(3, result.Count());
		Assert.Contains(result, smt => smt.Name == "Facebook");
		Assert.Contains(result, smt => smt.Name == "Instagram");
		Assert.Contains(result, smt => smt.Name == "LinkedIn");
	}

	[Fact]
	public async Task GetAllAsync_WhenNoSocialMediaTypes_ShouldReturnEmptyList()
	{
		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		Assert.Empty(result);
	}

	public void Dispose()
	{
		_context.Dispose();
	}
}
