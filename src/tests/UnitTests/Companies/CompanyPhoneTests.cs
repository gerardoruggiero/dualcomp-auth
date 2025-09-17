using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies;

public class CompanyPhoneTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateCompanyPhone()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var phoneTypeId = Guid.NewGuid();
		var phone = "+1234567890";
		var isPrimary = true;

		// Act
		var companyPhone = CompanyPhone.Create(companyId, phoneTypeId, phone, isPrimary);

		// Assert
		Assert.NotEqual(Guid.Empty, companyPhone.Id);
		Assert.Equal(companyId, companyPhone.CompanyId);
		Assert.Equal(phoneTypeId, companyPhone.PhoneTypeId);
		Assert.Equal(phone, companyPhone.Phone);
		Assert.Equal(isPrimary, companyPhone.IsPrimary);
	}

	[Fact]
	public void Create_WithNullPhoneType_ShouldThrowArgumentNullException()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var phone = "+1234567890";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => CompanyPhone.Create(companyId, Guid.Empty, phone));
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidPhone_ShouldThrowArgumentException(string? phone)
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var phoneTypeId = Guid.NewGuid();

		// Act & Assert
		Assert.Throws<ArgumentException>(() => CompanyPhone.Create(companyId, phoneTypeId, phone!));
	}

	[Fact]
	public void UpdatePhone_WithValidPhone_ShouldUpdatePhone()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var phoneTypeId = Guid.NewGuid();
		var originalPhone = "+1234567890";
		var newPhone = "+0987654321";
		var companyPhone = CompanyPhone.Create(companyId, phoneTypeId, originalPhone);

		// Act
		companyPhone.UpdatePhone(newPhone);

		// Assert
		Assert.Equal(newPhone, companyPhone.Phone);
	}

	[Fact]
	public void SetAsPrimary_ShouldSetIsPrimaryToTrue()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var phoneTypeId = Guid.NewGuid();
		var phone = "+1234567890";
		var companyPhone = CompanyPhone.Create(companyId, phoneTypeId, phone, false);

		// Act
		companyPhone.SetAsPrimary();

		// Assert
		Assert.True(companyPhone.IsPrimary);
	}

	[Fact]
	public void SetAsSecondary_ShouldSetIsPrimaryToFalse()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var phoneTypeId = Guid.NewGuid();
		var phone = "+1234567890";
		var companyPhone = CompanyPhone.Create(companyId, phoneTypeId, phone, true);

		// Act
		companyPhone.SetAsSecondary();

		// Assert
		Assert.False(companyPhone.IsPrimary);
	}
}
