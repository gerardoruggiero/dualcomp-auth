using DualComp.Infraestructure.Security;
using Moq;

namespace Dualcomp.Auth.UnitTests.Security;

public class PasswordGeneratorTests
{
	private readonly Mock<IPasswordValidator> _mockPasswordValidator;
	private readonly PasswordValidationSettings _settings;

	public PasswordGeneratorTests()
	{
		_mockPasswordValidator = new Mock<IPasswordValidator>();
		_settings = new PasswordValidationSettings
		{
			MinLength = 8,
			RequireUppercase = true,
			RequireLowercase = true,
			RequireDigit = true,
			RequireSpecialCharacter = true
		};
	}

	[Fact]
	public void GenerateTemporaryPassword_ShouldReturnValidPassword()
	{
		// Arrange
		_mockPasswordValidator.Setup(v => v.IsValid(It.IsAny<string>(), out It.Ref<string>.IsAny))
			.Returns(true);

		var generator = new PasswordGenerator(_settings, _mockPasswordValidator.Object);

		// Act
		var password = generator.GenerateTemporaryPassword();

		// Assert
		Assert.NotNull(password);
		Assert.NotEmpty(password);
		Assert.True(password.Length >= _settings.MinLength);
		Assert.Contains(password, c => char.IsUpper(c));
		Assert.Contains(password, c => char.IsLower(c));
		Assert.Contains(password, c => char.IsDigit(c));
		Assert.Contains(password, c => !char.IsLetterOrDigit(c));
	}

	[Fact]
	public void GenerateTemporaryPassword_ShouldRetryUntilValid()
	{
		// Arrange
		var callCount = 0;
		_mockPasswordValidator.Setup(v => v.IsValid(It.IsAny<string>(), out It.Ref<string>.IsAny))
			.Returns(() =>
			{
				callCount++;
				return callCount >= 3; // Valid on third attempt
			});

		var generator = new PasswordGenerator(_settings, _mockPasswordValidator.Object);

		// Act
		var password = generator.GenerateTemporaryPassword();

		// Assert
		Assert.NotNull(password);
		Assert.NotEmpty(password);
		Assert.True(callCount >= 3);
	}

	[Fact]
	public void GenerateTemporaryPassword_ShouldUseFallbackWhenMaxAttemptsReached()
	{
		// Arrange
		_mockPasswordValidator.Setup(v => v.IsValid(It.IsAny<string>(), out It.Ref<string>.IsAny))
			.Returns(false); // Always invalid

		var generator = new PasswordGenerator(_settings, _mockPasswordValidator.Object);

		// Act
		var password = generator.GenerateTemporaryPassword();

		// Assert
		Assert.NotNull(password);
		Assert.NotEmpty(password);
		Assert.True(password.Length >= _settings.MinLength);
		// Fallback password should still meet basic requirements
		Assert.Contains(password, c => char.IsUpper(c));
		Assert.Contains(password, c => char.IsLower(c));
		Assert.Contains(password, c => char.IsDigit(c));
		Assert.Contains(password, c => !char.IsLetterOrDigit(c));
	}

	[Fact]
	public void GenerateTemporaryPassword_WithMinimalSettings_ShouldWork()
	{
		// Arrange
		var minimalSettings = new PasswordValidationSettings
		{
			MinLength = 6,
			RequireUppercase = false,
			RequireLowercase = false,
			RequireDigit = false,
			RequireSpecialCharacter = false
		};

		_mockPasswordValidator.Setup(v => v.IsValid(It.IsAny<string>(), out It.Ref<string>.IsAny))
			.Returns(true);

		var generator = new PasswordGenerator(minimalSettings, _mockPasswordValidator.Object);

		// Act
		var password = generator.GenerateTemporaryPassword();

		// Assert
		Assert.NotNull(password);
		Assert.NotEmpty(password);
		Assert.True(password.Length >= minimalSettings.MinLength);
	}

	[Fact]
	public void GenerateTemporaryPassword_ShouldGenerateDifferentPasswords()
	{
		// Arrange
		_mockPasswordValidator.Setup(v => v.IsValid(It.IsAny<string>(), out It.Ref<string>.IsAny))
			.Returns(true);

		var generator = new PasswordGenerator(_settings, _mockPasswordValidator.Object);

		// Act
		var password1 = generator.GenerateTemporaryPassword();
		var password2 = generator.GenerateTemporaryPassword();

		// Assert
		Assert.NotEqual(password1, password2);
	}

	[Fact]
	public void GenerateTemporaryPassword_WithNullSettings_ShouldThrowArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new PasswordGenerator(null!, _mockPasswordValidator.Object));
	}

	[Fact]
	public void GenerateTemporaryPassword_WithNullValidator_ShouldThrowArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new PasswordGenerator(_settings, null!));
	}

	[Fact]
	public void GenerateTemporaryPassword_WithNoRequiredCharacters_ShouldStillWork()
	{
		// Arrange
		var noRequirementsSettings = new PasswordValidationSettings
		{
			MinLength = 6,
			RequireUppercase = false,
			RequireLowercase = false,
			RequireDigit = false,
			RequireSpecialCharacter = false
		};

		_mockPasswordValidator.Setup(v => v.IsValid(It.IsAny<string>(), out It.Ref<string>.IsAny))
			.Returns(true);

		var generator = new PasswordGenerator(noRequirementsSettings, _mockPasswordValidator.Object);

		// Act
		var password = generator.GenerateTemporaryPassword();

		// Assert
		Assert.NotNull(password);
		Assert.NotEmpty(password);
		Assert.True(password.Length >= noRequirementsSettings.MinLength);
	}
}
