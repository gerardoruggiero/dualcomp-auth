using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.DataAccess.EntityFramework.Repositories;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.UnitTests.DataAccess.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.UnitTests.DataAccess.Repositories;

public class ModuloRepositoryTests : IDisposable
{
	private readonly BaseDbContext _context;
	private readonly ModuloRepository _repository;

	public ModuloRepositoryTests()
	{
		var options = new DbContextOptionsBuilder<BaseDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_context = new BaseDbContext(options);
		var factoryMock = new DbContextFactoryMock(_context);
		_repository = new ModuloRepository(factoryMock, _context);
	}

	[Fact]
	public async Task GetAllAsync_Should_Return_All_Modulos()
	{
		// Arrange
		var modulos = new List<ModuloEntity>
		{
			ModuloEntity.Create("Usuarios", "Módulo de gestión de usuarios"),
			ModuloEntity.Create("Empresas", "Módulo de gestión de empresas"),
			ModuloEntity.Create("Reportes", "Módulo de reportes y estadísticas")
		};

		foreach (var modulo in modulos)
		{
			_context.Modulos.Add(modulo);
		}
		await _context.SaveChangesAsync();

		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		Assert.Equal(3, result.Count());
		Assert.Contains(result, t => t.Name == "Usuarios");
		Assert.Contains(result, t => t.Name == "Empresas");
		Assert.Contains(result, t => t.Name == "Reportes");
	}

	[Fact]
	public async Task AddAsync_Should_Add_Modulo()
	{
		// Arrange
		var modulo = ModuloEntity.Create("Usuarios", "Módulo de gestión de usuarios");

		// Act
		await _repository.AddAsync(modulo);
		await _context.SaveChangesAsync();

		// Assert
		var savedModulo = await _repository.GetByIdAsync(modulo.Id);
		Assert.NotNull(savedModulo);
		Assert.Equal("Usuarios", savedModulo.Name);
		Assert.Equal("Módulo de gestión de usuarios", savedModulo.Description);
	}

	[Fact]
	public async Task UpdateAsync_Should_Update_Modulo()
	{
		// Arrange
		var modulo = ModuloEntity.Create("Usuarios", "Original description");
		_context.Modulos.Add(modulo);
		await _context.SaveChangesAsync();

		// Act
		modulo.UpdateInfo("Updated Usuarios", "Updated description");
		await _repository.UpdateAsync(modulo);
		await _context.SaveChangesAsync();

		// Assert
		var updatedModulo = await _repository.GetByIdAsync(modulo.Id);
		Assert.NotNull(updatedModulo);
		Assert.Equal("Updated Usuarios", updatedModulo.Name);
		Assert.Equal("Updated description", updatedModulo.Description);
	}

	public void Dispose()
	{
		_context.Dispose();
	}
}
