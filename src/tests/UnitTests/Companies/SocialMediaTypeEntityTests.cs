using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies;

public class SocialMediaTypeEntityTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateSocialMediaTypeEntity()
	{
		// Arrange
		var name = "Facebook";
		var description = "Página de Facebook";

		// Act
		var socialMediaType = SocialMediaTypeEntity.Create(name, description);

		// Assert
		Assert.NotEqual(Guid.Empty, socialMediaType.Id);
		Assert.Equal(name, socialMediaType.Name);
		Assert.Equal(description, socialMediaType.Description);
		Assert.True(socialMediaType.IsActive);
	}

	[Fact]
	public void Create_WithOnlyName_ShouldCreateSocialMediaTypeEntity()
	{
		// Arrange
		var name = "Instagram";

		// Act
		var socialMediaType = SocialMediaTypeEntity.Create(name);

		// Assert
		Assert.NotEqual(Guid.Empty, socialMediaType.Id);
		Assert.Equal(name, socialMediaType.Name);
		Assert.Null(socialMediaType.Description);
		Assert.True(socialMediaType.IsActive);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => SocialMediaTypeEntity.Create(name!));
	}

	[Fact]
	public void UpdateInfo_WithValidData_ShouldUpdateInfo()
	{
		// Arrange
		var socialMediaType = SocialMediaTypeEntity.Create("Facebook", "Original description");
		var newName = "Updated Facebook";
		var newDescription = "Updated description";

		// Act
		socialMediaType.UpdateInfo(newName, newDescription);

		// Assert
		Assert.Equal(newName, socialMediaType.Name);
		Assert.Equal(newDescription, socialMediaType.Description);
	}

	[Fact]
	public void Activate_ShouldSetIsActiveToTrue()
	{
		// Arrange
		var socialMediaType = SocialMediaTypeEntity.Create("Facebook");
		socialMediaType.Deactivate(); // First deactivate

		// Act
		socialMediaType.Activate();

		// Assert
		Assert.True(socialMediaType.IsActive);
	}

	[Fact]
	public void Deactivate_ShouldSetIsActiveToFalse()
	{
		// Arrange
		var socialMediaType = SocialMediaTypeEntity.Create("Facebook");

		// Act
		socialMediaType.Deactivate();

		// Assert
		Assert.False(socialMediaType.IsActive);
	}
}
