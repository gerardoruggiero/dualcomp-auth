using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies;

public class PhoneTypeEntityTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreatePhoneTypeEntity()
	{
		// Arrange
		var name = "Principal";
		var description = "Teléfono principal de contacto";

		// Act
		var phoneType = PhoneTypeEntity.Create(name, description);

		// Assert
		Assert.NotEqual(Guid.Empty, phoneType.Id);
		Assert.Equal(name, phoneType.Name);
		Assert.Equal(description, phoneType.Description);
		Assert.True(phoneType.IsActive);
	}

	[Fact]
	public void Create_WithOnlyName_ShouldCreatePhoneTypeEntity()
	{
		// Arrange
		var name = "Móvil";

		// Act
		var phoneType = PhoneTypeEntity.Create(name);

		// Assert
		Assert.NotEqual(Guid.Empty, phoneType.Id);
		Assert.Equal(name, phoneType.Name);
		Assert.Null(phoneType.Description);
		Assert.True(phoneType.IsActive);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => PhoneTypeEntity.Create(name!));
	}

	[Fact]
	public void UpdateInfo_WithValidData_ShouldUpdateInfo()
	{
		// Arrange
		var phoneType = PhoneTypeEntity.Create("Principal", "Original description");
		var newName = "Updated Principal";
		var newDescription = "Updated description";

		// Act
		phoneType.UpdateInfo(newName, newDescription);

		// Assert
		Assert.Equal(newName, phoneType.Name);
		Assert.Equal(newDescription, phoneType.Description);
	}

	[Fact]
	public void Activate_ShouldSetIsActiveToTrue()
	{
		// Arrange
		var phoneType = PhoneTypeEntity.Create("Principal");
		phoneType.Deactivate(); // First deactivate

		// Act
		phoneType.Activate();

		// Assert
		Assert.True(phoneType.IsActive);
	}

	[Fact]
	public void Deactivate_ShouldSetIsActiveToFalse()
	{
		// Arrange
		var phoneType = PhoneTypeEntity.Create("Principal");

		// Act
		phoneType.Deactivate();

		// Assert
		Assert.False(phoneType.IsActive);
	}
}
