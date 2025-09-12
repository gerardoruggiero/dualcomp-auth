using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.UnitTests.Companies.ValueObjects;

public class PhoneTypeTests
{
	[Theory]
	[InlineData("Principal")]
	[InlineData("Móvil")]
	[InlineData("Fax")]
	[InlineData("WhatsApp")]
	public void Create_WithValidValue_ShouldReturnPhoneType(string value)
	{
		// Act
		var phoneType = PhoneType.Create(value);

		// Assert
		Assert.Equal(value, phoneType.Value);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidValue_ShouldThrowArgumentException(string? value)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => PhoneType.Create(value!));
	}

	[Theory]
	[InlineData("Invalid")]
	[InlineData("PRINCIPAL")]
	public void Create_WithInvalidType_ShouldThrowArgumentException(string value)
	{
		// Act & Assert
		var exception = Assert.Throws<ArgumentException>(() => PhoneType.Create(value));
		Assert.Contains("PhoneType must be one of", exception.Message);
	}

	[Theory]
	[InlineData("Principal ")]
	public void Create_WithTrailingWhitespace_ShouldNormalizeValue(string value)
	{
		// Act
		var phoneType = PhoneType.Create(value);

		// Assert
		Assert.Equal("Principal", phoneType.Value);
	}

	[Theory]
	[InlineData("Principal", "Principal")]
	[InlineData("Móvil", "Móvil")]
	public void Create_WithSameValue_ShouldBeEqual(string value1, string value2)
	{
		// Act
		var phoneType1 = PhoneType.Create(value1);
		var phoneType2 = PhoneType.Create(value2);

		// Assert
		Assert.Equal(phoneType1, phoneType2);
		Assert.Equal(phoneType1.Value, phoneType2.Value);
	}

	[Fact]
	public void Create_WithDifferentValues_ShouldNotBeEqual()
	{
		// Act
		var phoneType1 = PhoneType.Create("Principal");
		var phoneType2 = PhoneType.Create("Móvil");

		// Assert
		Assert.NotEqual(phoneType1, phoneType2);
		Assert.NotEqual(phoneType1.Value, phoneType2.Value);
	}

	[Fact]
	public void ToString_ShouldReturnValue()
	{
		// Arrange
		var value = "Principal";
		var phoneType = PhoneType.Create(value);

		// Act
		var result = phoneType.ToString();

		// Assert
		Assert.Equal(value, result);
	}
}
