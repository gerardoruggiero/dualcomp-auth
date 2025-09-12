using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.DataAccess.EntityFramework.Repositories;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.UnitTests.DataAccess.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.UnitTests.DataAccess.Repositories;

public class EmailTypeRepositoryTests : IDisposable
{
	private readonly BaseDbContext _context;
	private readonly EmailTypeRepository _repository;

	public EmailTypeRepositoryTests()
	{
		var options = new DbContextOptionsBuilder<BaseDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_context = new BaseDbContext(options);
		var factoryMock = new DbContextFactoryMock(_context);
		_repository = new EmailTypeRepository(factoryMock, _context);
	}

	[Fact]
	public async Task GetAllAsync_ShouldReturnAllEmailTypes()
	{
		// Arrange
		var emailTypes = new List<EmailTypeEntity>
		{
			EmailTypeEntity.Create("Principal", "Email principal"),
			EmailTypeEntity.Create("Facturación", "Email de facturación"),
			EmailTypeEntity.Create("Soporte", "Email de soporte")
		};

		foreach (var emailType in emailTypes)
		{
			_context.EmailTypes.Add(emailType);
		}
		await _context.SaveChangesAsync();

		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		Assert.Equal(3, result.Count());
		Assert.Contains(result, et => et.Name == "Principal");
		Assert.Contains(result, et => et.Name == "Facturación");
		Assert.Contains(result, et => et.Name == "Soporte");
	}

	[Fact]
	public async Task GetAllAsync_WhenNoEmailTypes_ShouldReturnEmptyList()
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
