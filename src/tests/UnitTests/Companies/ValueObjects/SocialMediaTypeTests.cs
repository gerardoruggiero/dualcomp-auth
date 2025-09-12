using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.UnitTests.Companies.ValueObjects;

public class SocialMediaTypeTests
{
	[Theory]
	[InlineData("Facebook")]
	[InlineData("Instagram")]
	[InlineData("LinkedIn")]
	[InlineData("Twitter")]
	[InlineData("YouTube")]
	public void Create_WithValidValue_ShouldReturnSocialMediaType(string value)
	{
		// Act
		var socialMediaType = SocialMediaType.Create(value);

		// Assert
		Assert.Equal(value, socialMediaType.Value);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Create_WithInvalidValue_ShouldThrowArgumentException(string? value)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => SocialMediaType.Create(value!));
	}

	[Theory]
	[InlineData("Invalid")]
	[InlineData("FACEBOOK")]
	public void Create_WithInvalidType_ShouldThrowArgumentException(string value)
	{
		// Act & Assert
		var exception = Assert.Throws<ArgumentException>(() => SocialMediaType.Create(value));
		Assert.Contains("SocialMediaType must be one of", exception.Message);
	}

	[Theory]
	[InlineData("Facebook ")]
	public void Create_WithTrailingWhitespace_ShouldNormalizeValue(string value)
	{
		// Act
		var socialMediaType = SocialMediaType.Create(value);

		// Assert
		Assert.Equal("Facebook", socialMediaType.Value);
	}

	[Theory]
	[InlineData("Facebook", "Facebook")]
	[InlineData("LinkedIn", "LinkedIn")]
	public void Create_WithSameValue_ShouldBeEqual(string value1, string value2)
	{
		// Act
		var socialMediaType1 = SocialMediaType.Create(value1);
		var socialMediaType2 = SocialMediaType.Create(value2);

		// Assert
		Assert.Equal(socialMediaType1, socialMediaType2);
		Assert.Equal(socialMediaType1.Value, socialMediaType2.Value);
	}

	[Fact]
	public void Create_WithDifferentValues_ShouldNotBeEqual()
	{
		// Act
		var socialMediaType1 = SocialMediaType.Create("Facebook");
		var socialMediaType2 = SocialMediaType.Create("LinkedIn");

		// Assert
		Assert.NotEqual(socialMediaType1, socialMediaType2);
		Assert.NotEqual(socialMediaType1.Value, socialMediaType2.Value);
	}

	[Fact]
	public void ToString_ShouldReturnValue()
	{
		// Arrange
		var value = "Facebook";
		var socialMediaType = SocialMediaType.Create(value);

		// Act
		var result = socialMediaType.ToString();

		// Assert
		Assert.Equal(value, result);
	}
}
