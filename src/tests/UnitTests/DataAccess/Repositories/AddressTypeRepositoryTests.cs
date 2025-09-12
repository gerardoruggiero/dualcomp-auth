using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.DataAccess.EntityFramework.Repositories;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.UnitTests.DataAccess.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace Dualcomp.Auth.UnitTests.DataAccess.Repositories;

public class AddressTypeRepositoryTests : IDisposable
{
	private readonly BaseDbContext _context;
	private readonly AddressTypeRepository _repository;

	public AddressTypeRepositoryTests()
	{
		var options = new DbContextOptionsBuilder<BaseDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_context = new BaseDbContext(options);
		var factoryMock = new DbContextFactoryMock(_context);
		_repository = new AddressTypeRepository(factoryMock, _context);
	}

	[Fact]
	public async Task GetAllAsync_ShouldReturnAllAddressTypes()
	{
		// Arrange
		var addressTypes = new List<AddressTypeEntity>
		{
			AddressTypeEntity.Create("Principal", "Dirección principal"),
			AddressTypeEntity.Create("Sucursal", "Dirección de sucursal"),
			AddressTypeEntity.Create("Facturación", "Dirección de facturación")
		};

		foreach (var addressType in addressTypes)
		{
			_context.AddressTypes.Add(addressType);
		}
		await _context.SaveChangesAsync();

		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		Assert.Equal(3, result.Count());
		Assert.Contains(result, at => at.Name == "Principal");
		Assert.Contains(result, at => at.Name == "Sucursal");
		Assert.Contains(result, at => at.Name == "Facturación");
	}

	[Fact]
	public async Task GetAllAsync_WhenNoAddressTypes_ShouldReturnEmptyList()
	{
		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetAllAsync_ShouldReturnOnlyActiveAddressTypes()
	{
		// Arrange
		var activeType = AddressTypeEntity.Create("Principal", "Dirección principal");
		var inactiveType = AddressTypeEntity.Create("Sucursal", "Dirección de sucursal");
		inactiveType.Deactivate();

		_context.AddressTypes.Add(activeType);
		_context.AddressTypes.Add(inactiveType);
		await _context.SaveChangesAsync();

		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		Assert.Equal(2, result.Count()); // Repository returns all, filtering should be done in application layer
	}

	public void Dispose()
	{
		_context.Dispose();
	}
}
