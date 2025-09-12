using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.UnitTests.Companies;

public class CompanyAddressTests
{
	[Fact]
	public void Create_WithValidData_ShouldCreateCompanyAddress()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var addressTypeId = Guid.NewGuid();
		var address = "123 Main St, City, Country";
		var isPrimary = true;

		// Act
		var companyAddress = CompanyAddress.Create(companyId, addressTypeId, address, isPrimary);

		// Assert
		Assert.NotEqual(Guid.Empty, companyAddress.Id);
		Assert.Equal(companyId, companyAddress.CompanyId);
		Assert.Equal(addressTypeId, companyAddress.AddressTypeId);
		Assert.Equal(address, companyAddress.Address);
		Assert.Equal(isPrimary, companyAddress.IsPrimary);
	}

	[Fact]
	public void Create_WithEmptyAddressTypeId_ShouldThrowArgumentException()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var address = "123 Main St, City, Country";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => CompanyAddress.Create(companyId, Guid.Empty, address));
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidAddress_ShouldThrowArgumentException(string? address)
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var addressTypeId = Guid.NewGuid();

		// Act & Assert
		Assert.Throws<ArgumentException>(() => CompanyAddress.Create(companyId, addressTypeId, address!));
	}

	[Fact]
	public void UpdateAddress_WithValidAddress_ShouldUpdateAddress()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var addressTypeId = Guid.NewGuid();
		var originalAddress = "123 Main St, City, Country";
		var newAddress = "456 New St, City, Country";
		var companyAddress = CompanyAddress.Create(companyId, addressTypeId, originalAddress);

		// Act
		companyAddress.UpdateAddress(newAddress);

		// Assert
		Assert.Equal(newAddress, companyAddress.Address);
	}

	[Fact]
	public void SetAsPrimary_ShouldSetIsPrimaryToTrue()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var addressTypeId = Guid.NewGuid();
		var address = "123 Main St, City, Country";
		var companyAddress = CompanyAddress.Create(companyId, addressTypeId, address, false);

		// Act
		companyAddress.SetAsPrimary();

		// Assert
		Assert.True(companyAddress.IsPrimary);
	}

	[Fact]
	public void SetAsSecondary_ShouldSetIsPrimaryToFalse()
	{
		// Arrange
		var companyId = Guid.NewGuid();
		var addressTypeId = Guid.NewGuid();
		var address = "123 Main St, City, Country";
		var companyAddress = CompanyAddress.Create(companyId, addressTypeId, address, true);

		// Act
		companyAddress.SetAsSecondary();

		// Assert
		Assert.False(companyAddress.IsPrimary);
	}
}
