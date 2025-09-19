using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies;

public class ModuloEntityTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateModuloEntity()
	{
		// Arrange
		var name = "Usuarios";
		var description = "Módulo de gestión de usuarios";

		// Act
		var modulo = ModuloEntity.Create(name, description);

		// Assert
		Assert.NotEqual(Guid.Empty, modulo.Id);
		Assert.Equal(name, modulo.Name);
		Assert.Equal(description, modulo.Description);
		Assert.True(modulo.IsActive);
	}

	[Fact]
	public void Create_WithOnlyName_ShouldCreateModuloEntity()
	{
		// Arrange
		var name = "Empresas";

		// Act
		var modulo = ModuloEntity.Create(name);

		// Assert
		Assert.NotEqual(Guid.Empty, modulo.Id);
		Assert.Equal(name, modulo.Name);
		Assert.Null(modulo.Description);
		Assert.True(modulo.IsActive);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => ModuloEntity.Create(name!));
	}

	[Fact]
	public void UpdateInfo_WithValidData_ShouldUpdateInfo()
	{
		// Arrange
		var modulo = ModuloEntity.Create("Usuarios", "Original description");
		var newName = "Updated Usuarios";
		var newDescription = "Updated description";

		// Act
		modulo.UpdateInfo(newName, newDescription);

		// Assert
		Assert.Equal(newName, modulo.Name);
		Assert.Equal(newDescription, modulo.Description);
	}

	[Fact]
	public void Activate_ShouldSetIsActiveToTrue()
	{
		// Arrange
		var modulo = ModuloEntity.Create("Usuarios");
		modulo.Deactivate(); // First deactivate

		// Act
		modulo.Activate();

		// Assert
		Assert.True(modulo.IsActive);
	}

	[Fact]
	public void Deactivate_ShouldSetIsActiveToFalse()
	{
		// Arrange
		var modulo = ModuloEntity.Create("Usuarios");

		// Act
		modulo.Deactivate();

		// Assert
		Assert.False(modulo.IsActive);
	}
}
