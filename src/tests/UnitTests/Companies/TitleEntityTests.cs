using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies;

public class TitleEntityTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateTitleEntity()
	{
		// Arrange
		var name = "Ingeniero";
		var description = "Título de Ingeniero";

		// Act
		var title = TitleEntity.Create(name, description);

		// Assert
		Assert.NotEqual(Guid.Empty, title.Id);
		Assert.Equal(name, title.Name);
		Assert.Equal(description, title.Description);
		Assert.True(title.IsActive);
	}

	[Fact]
	public void Create_WithOnlyName_ShouldCreateTitleEntity()
	{
		// Arrange
		var name = "Doctor";

		// Act
		var title = TitleEntity.Create(name);

		// Assert
		Assert.NotEqual(Guid.Empty, title.Id);
		Assert.Equal(name, title.Name);
		Assert.Null(title.Description);
		Assert.True(title.IsActive);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => TitleEntity.Create(name!));
	}

	[Fact]
	public void UpdateInfo_WithValidData_ShouldUpdateInfo()
	{
		// Arrange
		var title = TitleEntity.Create("Ingeniero", "Original description");
		var newName = "Updated Ingeniero";
		var newDescription = "Updated description";

		// Act
		title.UpdateInfo(newName, newDescription);

		// Assert
		Assert.Equal(newName, title.Name);
		Assert.Equal(newDescription, title.Description);
	}

	[Fact]
	public void Activate_ShouldSetIsActiveToTrue()
	{
		// Arrange
		var title = TitleEntity.Create("Ingeniero");
		title.Deactivate(); // First deactivate

		// Act
		title.Activate();

		// Assert
		Assert.True(title.IsActive);
	}

	[Fact]
	public void Deactivate_ShouldSetIsActiveToFalse()
	{
		// Arrange
		var title = TitleEntity.Create("Ingeniero");

		// Act
		title.Deactivate();

		// Assert
		Assert.False(title.IsActive);
	}
}

