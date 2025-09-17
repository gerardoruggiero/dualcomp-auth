using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies;

public class CompanySocialMediaTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateCompanySocialMedia()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var socialMediaTypeId = Guid.NewGuid();
		var url = "https://facebook.com/company";
		var isPrimary = true;

		// Act
		var companySocialMedia = CompanySocialMedia.Create(companyId, socialMediaTypeId, url, isPrimary);

		// Assert
		Assert.NotEqual(Guid.Empty, companySocialMedia.Id);
		Assert.Equal(companyId, companySocialMedia.CompanyId);
		Assert.Equal(socialMediaTypeId, companySocialMedia.SocialMediaTypeId);
		Assert.Equal(url, companySocialMedia.Url);
		Assert.Equal(isPrimary, companySocialMedia.IsPrimary);
	}

	[Fact]
	public void Create_WithNullSocialMediaType_ShouldThrowArgumentNullException()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var url = "https://facebook.com/company";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => CompanySocialMedia.Create(companyId, Guid.Empty, url));
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidUrl_ShouldThrowArgumentException(string? url)
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var socialMediaTypeId = Guid.NewGuid();

		// Act & Assert
		Assert.Throws<ArgumentException>(() => CompanySocialMedia.Create(companyId, socialMediaTypeId, url!));
	}

	[Fact]
	public void UpdateUrl_WithValidUrl_ShouldUpdateUrl()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var socialMediaTypeId = Guid.NewGuid();
		var originalUrl = "https://facebook.com/company";
		var newUrl = "https://facebook.com/newcompany";
		var companySocialMedia = CompanySocialMedia.Create(companyId, socialMediaTypeId, originalUrl);

		// Act
		companySocialMedia.UpdateUrl(newUrl);

		// Assert
		Assert.Equal(newUrl, companySocialMedia.Url);
	}

	[Fact]
	public void SetAsPrimary_ShouldSetIsPrimaryToTrue()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var socialMediaTypeId = Guid.NewGuid();
		var url = "https://facebook.com/company";
		var companySocialMedia = CompanySocialMedia.Create(companyId, socialMediaTypeId, url, false);

		// Act
		companySocialMedia.SetAsPrimary();

		// Assert
		Assert.True(companySocialMedia.IsPrimary);
	}

	[Fact]
	public void SetAsSecondary_ShouldSetIsPrimaryToFalse()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var socialMediaTypeId = Guid.NewGuid();
		var url = "https://facebook.com/company";
		var companySocialMedia = CompanySocialMedia.Create(companyId, socialMediaTypeId, url, true);

		// Act
		companySocialMedia.SetAsSecondary();

		// Assert
		Assert.False(companySocialMedia.IsPrimary);
	}
}
