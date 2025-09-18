using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.DataAccess.EntityFramework.Repositories;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.UnitTests.DataAccess.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.UnitTests.DataAccess.Repositories;

public class TitleRepositoryTests : IDisposable
{
	private readonly BaseDbContext _context;
	private readonly TitleRepository _repository;

	public TitleRepositoryTests()
	{
		var options = new DbContextOptionsBuilder<BaseDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_context = new BaseDbContext(options);
		var factoryMock = new DbContextFactoryMock(_context);
		_repository = new TitleRepository(factoryMock, _context);
	}

	[Fact]
	public async Task GetAllAsync_Should_Return_All_Titles()
	{
		// Arrange
		var titles = new List<TitleEntity>
		{
			TitleEntity.Create("Ingeniero", "Título de Ingeniero"),
			TitleEntity.Create("Doctor", "Título de Doctor"),
			TitleEntity.Create("Licenciado", "Título de Licenciado")
		};

		foreach (var title in titles)
		{
			_context.Titles.Add(title);
		}
		await _context.SaveChangesAsync();

		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		Assert.Equal(3, result.Count());
		Assert.Contains(result, t => t.Name == "Ingeniero");
		Assert.Contains(result, t => t.Name == "Doctor");
		Assert.Contains(result, t => t.Name == "Licenciado");
	}

	[Fact]
	public async Task AddAsync_Should_Add_Title()
	{
		// Arrange
		var title = TitleEntity.Create("Ingeniero", "Título de Ingeniero");

		// Act
		await _repository.AddAsync(title);
		await _context.SaveChangesAsync();

		// Assert
		var savedTitle = await _repository.GetByIdAsync(title.Id);
		Assert.NotNull(savedTitle);
		Assert.Equal("Ingeniero", savedTitle.Name);
		Assert.Equal("Título de Ingeniero", savedTitle.Description);
	}

	[Fact]
	public async Task UpdateAsync_Should_Update_Title()
	{
		// Arrange
		var title = TitleEntity.Create("Ingeniero", "Original description");
		_context.Titles.Add(title);
		await _context.SaveChangesAsync();

		// Act
		title.UpdateInfo("Updated Ingeniero", "Updated description");
		await _repository.UpdateAsync(title);
		await _context.SaveChangesAsync();

		// Assert
		var updatedTitle = await _repository.GetByIdAsync(title.Id);
		Assert.NotNull(updatedTitle);
		Assert.Equal("Updated Ingeniero", updatedTitle.Name);
		Assert.Equal("Updated description", updatedTitle.Description);
	}

	public void Dispose()
	{
		_context.Dispose();
	}
}

