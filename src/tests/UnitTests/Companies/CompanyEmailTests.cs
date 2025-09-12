using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.UnitTests.Companies;

public class CompanyEmailTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateCompanyEmail()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var emailTypeId = Guid.NewGuid();
		var email = Email.Create("test@company.com");
		var isPrimary = true;

		// Act
		var companyEmail = CompanyEmail.Create(companyId, emailTypeId, email, isPrimary);

		// Assert
		Assert.NotEqual(Guid.Empty, companyEmail.Id);
		Assert.Equal(companyId, companyEmail.CompanyId);
		Assert.Equal(emailTypeId, companyEmail.EmailTypeId);
		Assert.Equal(email, companyEmail.Email);
		Assert.Equal(isPrimary, companyEmail.IsPrimary);
	}

	[Fact]
	public void Create_WithEmptyEmailTypeId_ShouldThrowArgumentException()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var email = Email.Create("test@company.com");

		// Act & Assert
		Assert.Throws<ArgumentException>(() => CompanyEmail.Create(companyId, Guid.Empty, email));
	}

	[Fact]
	public void Create_WithNullEmail_ShouldThrowArgumentNullException()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var emailTypeId = Guid.NewGuid();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => CompanyEmail.Create(companyId, emailTypeId, null!));
	}

	[Fact]
	public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var emailTypeId = Guid.NewGuid();
		var originalEmail = Email.Create("old@company.com");
		var newEmail = Email.Create("new@company.com");
		var companyEmail = CompanyEmail.Create(companyId, emailTypeId, originalEmail);

		// Act
		companyEmail.UpdateEmail(newEmail);

		// Assert
		Assert.Equal(newEmail, companyEmail.Email);
	}

	[Fact]
	public void SetAsPrimary_ShouldSetIsPrimaryToTrue()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var emailTypeId = Guid.NewGuid();
		var email = Email.Create("test@company.com");
		var companyEmail = CompanyEmail.Create(companyId, emailTypeId, email, false);

		// Act
		companyEmail.SetAsPrimary();

		// Assert
		Assert.True(companyEmail.IsPrimary);
	}

	[Fact]
	public void SetAsSecondary_ShouldSetIsPrimaryToFalse()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var emailTypeId = Guid.NewGuid();
		var email = Email.Create("test@company.com");
		var companyEmail = CompanyEmail.Create(companyId, emailTypeId, email, true);

		// Act
		companyEmail.SetAsSecondary();

		// Assert
		Assert.False(companyEmail.IsPrimary);
	}
}
