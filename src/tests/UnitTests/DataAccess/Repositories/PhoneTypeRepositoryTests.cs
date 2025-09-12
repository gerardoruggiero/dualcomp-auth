using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.DataAccess.EntityFramework.Repositories;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.UnitTests.DataAccess.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.UnitTests.DataAccess.Repositories;

public class PhoneTypeRepositoryTests : IDisposable
{
	private readonly BaseDbContext _context;
	private readonly PhoneTypeRepository _repository;

	public PhoneTypeRepositoryTests()
	{
		var options = new DbContextOptionsBuilder<BaseDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_context = new BaseDbContext(options);
		var factoryMock = new DbContextFactoryMock(_context);
		_repository = new PhoneTypeRepository(factoryMock, _context);
	}

	[Fact]
	public async Task GetAllAsync_ShouldReturnAllPhoneTypes()
	{
		// Arrange
		var phoneTypes = new List<PhoneTypeEntity>
		{
			PhoneTypeEntity.Create("Principal", "Teléfono principal"),
			PhoneTypeEntity.Create("Móvil", "Teléfono móvil"),
			PhoneTypeEntity.Create("Fax", "Número de fax")
		};

		foreach (var phoneType in phoneTypes)
		{
			_context.PhoneTypes.Add(phoneType);
		}
		await _context.SaveChangesAsync();

		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		Assert.Equal(3, result.Count());
		Assert.Contains(result, pt => pt.Name == "Principal");
		Assert.Contains(result, pt => pt.Name == "Móvil");
		Assert.Contains(result, pt => pt.Name == "Fax");
	}

	[Fact]
	public async Task GetAllAsync_WhenNoPhoneTypes_ShouldReturnEmptyList()
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
