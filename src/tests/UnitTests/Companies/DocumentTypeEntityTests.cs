using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies;

public class DocumentTypeEntityTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateDocumentTypeEntity()
	{
		// Arrange
		var name = "DNI";
		var description = "Documento Nacional de Identidad";

		// Act
		var documentType = DocumentTypeEntity.Create(name, description);

		// Assert
		Assert.NotEqual(Guid.Empty, documentType.Id);
		Assert.Equal(name, documentType.Name);
		Assert.Equal(description, documentType.Description);
		Assert.True(documentType.IsActive);
	}

	[Fact]
	public void Create_WithOnlyName_ShouldCreateDocumentTypeEntity()
	{
		// Arrange
		var name = "Pasaporte";

		// Act
		var documentType = DocumentTypeEntity.Create(name);

		// Assert
		Assert.NotEqual(Guid.Empty, documentType.Id);
		Assert.Equal(name, documentType.Name);
		Assert.Null(documentType.Description);
		Assert.True(documentType.IsActive);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => DocumentTypeEntity.Create(name!));
	}

	[Fact]
	public void UpdateInfo_WithValidData_ShouldUpdateInfo()
	{
		// Arrange
		var documentType = DocumentTypeEntity.Create("DNI", "Original description");
		var newName = "Updated DNI";
		var newDescription = "Updated description";

		// Act
		documentType.UpdateInfo(newName, newDescription);

		// Assert
		Assert.Equal(newName, documentType.Name);
		Assert.Equal(newDescription, documentType.Description);
	}

	[Fact]
	public void Activate_ShouldSetIsActiveToTrue()
	{
		// Arrange
		var documentType = DocumentTypeEntity.Create("DNI");
		documentType.Deactivate(); // First deactivate

		// Act
		documentType.Activate();

		// Assert
		Assert.True(documentType.IsActive);
	}

	[Fact]
	public void Deactivate_ShouldSetIsActiveToFalse()
	{
		// Arrange
		var documentType = DocumentTypeEntity.Create("DNI");

		// Act
		documentType.Deactivate();

		// Assert
		Assert.False(documentType.IsActive);
	}
}

