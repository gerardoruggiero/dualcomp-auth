using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.UnitTests.Companies.ValueObjects;

public class EmailTypeTests
{
	[Theory]
	[InlineData("Principal")]
	[InlineData("Facturación")]
	[InlineData("Soporte")]
	[InlineData("Comercial")]
	public void Create_WithValidValue_ShouldReturnEmailType(string value)
	{
		// Act
		var emailType = EmailType.Create(value);

		// Assert
		Assert.Equal(value, emailType.Value);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidValue_ShouldThrowArgumentException(string? value)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => EmailType.Create(value!));
	}

	[Theory]
	[InlineData("Invalid")]
	[InlineData("PRINCIPAL")]
	public void Create_WithInvalidType_ShouldThrowArgumentException(string value)
	{
		// Act & Assert
		var exception = Assert.Throws<ArgumentException>(() => EmailType.Create(value));
		Assert.Contains("EmailType must be one of", exception.Message);
	}

	[Theory]
	[InlineData("Principal ")]
	public void Create_WithTrailingWhitespace_ShouldNormalizeValue(string value)
	{
		// Act
		var emailType = EmailType.Create(value);

		// Assert
		Assert.Equal("Principal", emailType.Value);
	}

	[Theory]
	[InlineData("Principal", "Principal")]
	[InlineData("Soporte", "Soporte")]
	public void Create_WithSameValue_ShouldBeEqual(string value1, string value2)
	{
		// Act
		var emailType1 = EmailType.Create(value1);
		var emailType2 = EmailType.Create(value2);

		// Assert
		Assert.Equal(emailType1, emailType2);
		Assert.Equal(emailType1.Value, emailType2.Value);
	}

	[Fact]
	public void Create_WithDifferentValues_ShouldNotBeEqual()
	{
		// Act
		var emailType1 = EmailType.Create("Principal");
		var emailType2 = EmailType.Create("Soporte");

		// Assert
		Assert.NotEqual(emailType1, emailType2);
		Assert.NotEqual(emailType1.Value, emailType2.Value);
	}

	[Fact]
	public void ToString_ShouldReturnValue()
	{
		// Arrange
		var value = "Principal";
		var emailType = EmailType.Create(value);

		// Act
		var result = emailType.ToString();

		// Assert
		Assert.Equal(value, result);
	}
}
