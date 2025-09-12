using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.UnitTests.Companies;

public class CompanyTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateCompany()
	{
		// Arrange
		var name = "Test Company";
		var taxId = TaxId.Create("12345678-9");

		// Act
		var company = Company.Create(name, taxId);

		// Assert
		Assert.NotEqual(Guid.Empty, company.Id);
		Assert.Equal(name, company.Name);
		Assert.Equal(taxId, company.TaxId);
		Assert.Empty(company.Addresses);
		Assert.Empty(company.Emails);
		Assert.Empty(company.Phones);
		Assert.Empty(company.SocialMedias);
		Assert.Empty(company.Employees);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
	{
		// Arrange
		var taxId = TaxId.Create("12345678-9");

		// Act & Assert
		Assert.Throws<ArgumentException>(() => Company.Create(name!, taxId));
	}

	[Fact]
	public void Create_WithNullTaxId_ShouldThrowArgumentNullException()
	{
		// Arrange
		var name = "Test Company";

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => Company.Create(name, null!));
	}

	[Fact]
	public void AddAddress_WithValidAddress_ShouldAddToCollection()
	{
		// Arrange
		var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
		var addressTypeId = Guid.NewGuid();
		var address = CompanyAddress.Create(company.Id, addressTypeId, "123 Main St");

		// Act
		company.AddAddress(address);

		// Assert
		Assert.Single(company.Addresses);
		Assert.Contains(address, company.Addresses);
	}

	[Fact]
	public void AddEmail_WithValidEmail_ShouldAddToCollection()
	{
		// Arrange
		var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
		var emailTypeId = Guid.NewGuid();
		var email = Email.Create("test@company.com");
		var companyEmail = CompanyEmail.Create(company.Id, emailTypeId, email);

		// Act
		company.AddEmail(companyEmail);

		// Assert
		Assert.Single(company.Emails);
		Assert.Contains(companyEmail, company.Emails);
	}

	[Fact]
	public void AddPhone_WithValidPhone_ShouldAddToCollection()
	{
		// Arrange
		var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
		var phoneTypeId = Guid.NewGuid();
		var phone = CompanyPhone.Create(company.Id, phoneTypeId, "+1234567890");

		// Act
		company.AddPhone(phone);

		// Assert
		Assert.Single(company.Phones);
		Assert.Contains(phone, company.Phones);
	}

	[Fact]
	public void AddSocialMedia_WithValidSocialMedia_ShouldAddToCollection()
	{
		// Arrange
		var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
		var socialMediaTypeId = Guid.NewGuid();
		var socialMedia = CompanySocialMedia.Create(company.Id, socialMediaTypeId, "https://facebook.com/company");

		// Act
		company.AddSocialMedia(socialMedia);

		// Assert
		Assert.Single(company.SocialMedias);
		Assert.Contains(socialMedia, company.SocialMedias);
	}

	[Fact]
	public void AddEmployee_WithValidEmployee_ShouldAddToCollection()
	{
		// Arrange
		var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
		var employee = Employee.Create("John Doe", "john@company.com", "+1234567890", company.Id);

		// Act
		company.AddEmployee(employee);

		// Assert
		Assert.Single(company.Employees);
		Assert.Contains(employee, company.Employees);
	}

	[Fact]
	public void RemoveAddress_WithExistingAddress_ShouldRemoveFromCollection()
	{
		// Arrange
		var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
		var addressTypeId = Guid.NewGuid();
		var address = CompanyAddress.Create(company.Id, addressTypeId, "123 Main St");
		company.AddAddress(address);

		// Act
		company.RemoveAddress(address);

		// Assert
		Assert.Empty(company.Addresses);
	}

	[Fact]
	public void HasAtLeastOneAddress_WithNoAddresses_ShouldReturnFalse()
	{
		// Arrange
		var company = Company.Create("Test Company", TaxId.Create("12345678-9"));

		// Act
		var result = company.HasAtLeastOneAddress();

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void HasAtLeastOneAddress_WithAddresses_ShouldReturnTrue()
	{
		// Arrange
		var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
		var addressTypeId = Guid.NewGuid();
		var address = CompanyAddress.Create(company.Id, addressTypeId, "123 Main St");
		company.AddAddress(address);

		// Act
		var result = company.HasAtLeastOneAddress();

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void IsValidForRegistration_WithAllRequiredElements_ShouldReturnTrue()
	{
		// Arrange
		var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
		var addressTypeId = Guid.NewGuid();
		var address = CompanyAddress.Create(company.Id, addressTypeId, "123 Main St");
		company.AddAddress(address);

		var emailTypeId = Guid.NewGuid();
		var email = Email.Create("test@company.com");
		var companyEmail = CompanyEmail.Create(company.Id, emailTypeId, email);
		company.AddEmail(companyEmail);

		var phoneTypeId = Guid.NewGuid();
		var phone = CompanyPhone.Create(company.Id, phoneTypeId, "+1234567890");
		company.AddPhone(phone);

		var socialMediaTypeId = Guid.NewGuid();
		var socialMedia = CompanySocialMedia.Create(company.Id, socialMediaTypeId, "https://facebook.com/company");
		company.AddSocialMedia(socialMedia);

		var employee = Employee.Create("John Doe", "john@company.com", "+1234567890", company.Id);
		company.AddEmployee(employee);

		// Act
		var result = company.IsValidForRegistration();

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void IsValidForRegistration_WithMissingElements_ShouldReturnFalse()
	{
		// Arrange
		var company = Company.Create("Test Company", TaxId.Create("12345678-9"));

		// Act
		var result = company.IsValidForRegistration();

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void UpdateInfo_WithValidData_ShouldUpdateCompany()
	{
		// Arrange
		var company = Company.Create("Old Name", TaxId.Create("12345678-9"));
		var newName = "New Name";
		var newTaxId = TaxId.Create("98765432-1");

		// Act
		company.UpdateInfo(newName, newTaxId);

		// Assert
		Assert.Equal(newName, company.Name);
		Assert.Equal(newTaxId, company.TaxId);
	}
}
