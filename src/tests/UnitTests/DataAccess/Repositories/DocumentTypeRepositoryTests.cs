using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.DataAccess.EntityFramework.Repositories;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.UnitTests.DataAccess.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.UnitTests.DataAccess.Repositories;

public class DocumentTypeRepositoryTests : IDisposable
{
	private readonly BaseDbContext _context;
	private readonly DocumentTypeRepository _repository;

	public DocumentTypeRepositoryTests()
	{
		var options = new DbContextOptionsBuilder<BaseDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_context = new BaseDbContext(options);
		var factoryMock = new DbContextFactoryMock(_context);
		_repository = new DocumentTypeRepository(factoryMock, _context);
	}

	[Fact]
	public async Task GetAllAsync_Should_Return_All_DocumentTypes()
	{
		// Arrange
		var documentTypes = new List<DocumentTypeEntity>
		{
			DocumentTypeEntity.Create("DNI", "Documento Nacional de Identidad"),
			DocumentTypeEntity.Create("Pasaporte", "Pasaporte"),
			DocumentTypeEntity.Create("Cédula", "Cédula de Identidad")
		};

		foreach (var documentType in documentTypes)
		{
			_context.DocumentTypes.Add(documentType);
		}
		await _context.SaveChangesAsync();

		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		Assert.Equal(3, result.Count());
		Assert.Contains(result, t => t.Name == "DNI");
		Assert.Contains(result, t => t.Name == "Pasaporte");
		Assert.Contains(result, t => t.Name == "Cédula");
	}

	[Fact]
	public async Task AddAsync_Should_Add_DocumentType()
	{
		// Arrange
		var documentType = DocumentTypeEntity.Create("DNI", "Documento Nacional de Identidad");

		// Act
		await _repository.AddAsync(documentType);
		await _context.SaveChangesAsync();

		// Assert
		var savedDocumentType = await _repository.GetByIdAsync(documentType.Id);
		Assert.NotNull(savedDocumentType);
		Assert.Equal("DNI", savedDocumentType.Name);
		Assert.Equal("Documento Nacional de Identidad", savedDocumentType.Description);
	}

	[Fact]
	public async Task UpdateAsync_Should_Update_DocumentType()
	{
		// Arrange
		var documentType = DocumentTypeEntity.Create("DNI", "Original description");
		_context.DocumentTypes.Add(documentType);
		await _context.SaveChangesAsync();

		// Act
		documentType.UpdateInfo("Updated DNI", "Updated description");
		await _repository.UpdateAsync(documentType);
		await _context.SaveChangesAsync();

		// Assert
		var updatedDocumentType = await _repository.GetByIdAsync(documentType.Id);
		Assert.NotNull(updatedDocumentType);
		Assert.Equal("Updated DNI", updatedDocumentType.Name);
		Assert.Equal("Updated description", updatedDocumentType.Description);
	}

	public void Dispose()
	{
		_context.Dispose();
	}
}

