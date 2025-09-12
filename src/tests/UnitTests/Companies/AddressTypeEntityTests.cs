using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies;

public class AddressTypeEntityTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateAddressTypeEntity()
	{
		// Arrange
		var name = "Principal";
		var description = "Dirección principal de la empresa";

		// Act
		var addressType = AddressTypeEntity.Create(name, description);

		// Assert
		Assert.NotEqual(Guid.Empty, addressType.Id);
		Assert.Equal(name, addressType.Name);
		Assert.Equal(description, addressType.Description);
		Assert.True(addressType.IsActive);
	}

	[Fact]
	public void Create_WithOnlyName_ShouldCreateAddressTypeEntity()
	{
		// Arrange
		var name = "Sucursal";

		// Act
		var addressType = AddressTypeEntity.Create(name);

		// Assert
		Assert.NotEqual(Guid.Empty, addressType.Id);
		Assert.Equal(name, addressType.Name);
		Assert.Null(addressType.Description);
		Assert.True(addressType.IsActive);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => AddressTypeEntity.Create(name!));
	}

	[Fact]
	public void UpdateInfo_WithValidData_ShouldUpdateInfo()
	{
		// Arrange
		var addressType = AddressTypeEntity.Create("Principal", "Original description");
		var newName = "Updated Principal";
		var newDescription = "Updated description";

		// Act
		addressType.UpdateInfo(newName, newDescription);

		// Assert
		Assert.Equal(newName, addressType.Name);
		Assert.Equal(newDescription, addressType.Description);
	}

	[Fact]
	public void UpdateInfo_WithNullDescription_ShouldSetDescriptionToNull()
	{
		// Arrange
		var addressType = AddressTypeEntity.Create("Principal", "Original description");
		var newName = "Updated Principal";

		// Act
		addressType.UpdateInfo(newName, null);

		// Assert
		Assert.Equal(newName, addressType.Name);
		Assert.Null(addressType.Description);
	}

	[Fact]
	public void Activate_ShouldSetIsActiveToTrue()
	{
		// Arrange
		var addressType = AddressTypeEntity.Create("Principal");
		addressType.Deactivate(); // First deactivate

		// Act
		addressType.Activate();

		// Assert
		Assert.True(addressType.IsActive);
	}

	[Fact]
	public void Deactivate_ShouldSetIsActiveToFalse()
	{
		// Arrange
		var addressType = AddressTypeEntity.Create("Principal");

		// Act
		addressType.Deactivate();

		// Assert
		Assert.False(addressType.IsActive);
	}
}
