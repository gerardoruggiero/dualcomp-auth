using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.UnitTests.Companies.ValueObjects;

public class AddressTypeTests
{
	[Theory]
	[InlineData("Principal")]
	[InlineData("Sucursal")]
	[InlineData("Facturación")]
	[InlineData("Envío")]
	public void Create_WithValidValue_ShouldReturnAddressType(string value)
	{
		// Act
		var addressType = AddressType.Create(value);

		// Assert
		Assert.Equal(value, addressType.Value);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidValue_ShouldThrowArgumentException(string? value)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => AddressType.Create(value!));
	}

	[Theory]
	[InlineData("Invalid")]
	[InlineData("PRINCIPAL")]
	public void Create_WithInvalidType_ShouldThrowArgumentException(string value)
	{
		// Act & Assert
		var exception = Assert.Throws<ArgumentException>(() => AddressType.Create(value));
		Assert.Contains("AddressType must be one of", exception.Message);
	}

	[Theory]
	[InlineData("Principal ")]
	public void Create_WithTrailingWhitespace_ShouldNormalizeValue(string value)
	{
		// Act
		var addressType = AddressType.Create(value);

		// Assert
		Assert.Equal("Principal", addressType.Value);
	}

	[Theory]
	[InlineData("Principal", "Principal")]
	[InlineData("Sucursal", "Sucursal")]
	public void Create_WithSameValue_ShouldBeEqual(string value1, string value2)
	{
		// Act
		var addressType1 = AddressType.Create(value1);
		var addressType2 = AddressType.Create(value2);

		// Assert
		Assert.Equal(addressType1, addressType2);
		Assert.Equal(addressType1.Value, addressType2.Value);
	}

	[Fact]
	public void Create_WithDifferentValues_ShouldNotBeEqual()
	{
		// Act
		var addressType1 = AddressType.Create("Principal");
		var addressType2 = AddressType.Create("Sucursal");

		// Assert
		Assert.NotEqual(addressType1, addressType2);
		Assert.NotEqual(addressType1.Value, addressType2.Value);
	}

	[Fact]
	public void ToString_ShouldReturnValue()
	{
		// Arrange
		var value = "Principal";
		var addressType = AddressType.Create(value);

		// Act
		var result = addressType.ToString();

		// Assert
		Assert.Equal(value, result);
	}
}
