using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies;

public class EmailTypeEntityTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateEmailTypeEntity()
	{
		// Arrange
		var name = "Principal";
		var description = "Email principal de contacto";

		// Act
		var emailType = EmailTypeEntity.Create(name, description);

		// Assert
		Assert.NotEqual(Guid.Empty, emailType.Id);
		Assert.Equal(name, emailType.Name);
		Assert.Equal(description, emailType.Description);
		Assert.True(emailType.IsActive);
	}

	[Fact]
	public void Create_WithOnlyName_ShouldCreateEmailTypeEntity()
	{
		// Arrange
		var name = "Facturación";

		// Act
		var emailType = EmailTypeEntity.Create(name);

		// Assert
		Assert.NotEqual(Guid.Empty, emailType.Id);
		Assert.Equal(name, emailType.Name);
		Assert.Null(emailType.Description);
		Assert.True(emailType.IsActive);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => EmailTypeEntity.Create(name!));
	}

	[Fact]
	public void UpdateInfo_WithValidData_ShouldUpdateInfo()
	{
		// Arrange
		var emailType = EmailTypeEntity.Create("Principal", "Original description");
		var newName = "Updated Principal";
		var newDescription = "Updated description";

		// Act
		emailType.UpdateInfo(newName, newDescription);

		// Assert
		Assert.Equal(newName, emailType.Name);
		Assert.Equal(newDescription, emailType.Description);
	}

	[Fact]
	public void Activate_ShouldSetIsActiveToTrue()
	{
		// Arrange
		var emailType = EmailTypeEntity.Create("Principal");
		emailType.Deactivate(); // First deactivate

		// Act
		emailType.Activate();

		// Assert
		Assert.True(emailType.IsActive);
	}

	[Fact]
	public void Deactivate_ShouldSetIsActiveToFalse()
	{
		// Arrange
		var emailType = EmailTypeEntity.Create("Principal");

		// Act
		emailType.Deactivate();

		// Assert
		Assert.False(emailType.IsActive);
	}
}
