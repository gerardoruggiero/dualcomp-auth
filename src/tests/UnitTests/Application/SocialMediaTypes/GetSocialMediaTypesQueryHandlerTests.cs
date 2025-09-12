using Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes;
using Dualcomp.Auth.Domain.Companies;
using Moq;

namespace Dualcomp.Auth.UnitTests.Application.SocialMediaTypes;

public class GetSocialMediaTypesQueryHandlerTests
{
	[Fact]
	public async Task Handle_Should_Return_All_SocialMediaTypes()
	{
		// Arrange
		var mockRepository = new Mock<ISocialMediaTypeRepository>();
		var expectedTypes = new List<SocialMediaTypeEntity>
		{
			SocialMediaTypeEntity.Create("Facebook", "Página de Facebook"),
			SocialMediaTypeEntity.Create("Instagram", "Perfil de Instagram"),
			SocialMediaTypeEntity.Create("LinkedIn", "Página de LinkedIn"),
			SocialMediaTypeEntity.Create("Twitter", "Perfil de Twitter"),
			SocialMediaTypeEntity.Create("YouTube", "Canal de YouTube")
		};
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedTypes);

		var handler = new GetSocialMediaTypesQueryHandler(mockRepository.Object);
		var query = new GetSocialMediaTypesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(5, result.SocialMediaTypes.Count());
		Assert.Contains(result.SocialMediaTypes, t => t.Value == "Facebook");
		Assert.Contains(result.SocialMediaTypes, t => t.Value == "Instagram");
		Assert.Contains(result.SocialMediaTypes, t => t.Value == "LinkedIn");
		Assert.Contains(result.SocialMediaTypes, t => t.Value == "Twitter");
		Assert.Contains(result.SocialMediaTypes, t => t.Value == "YouTube");
	}

	[Fact]
	public async Task Handle_Should_Return_Empty_List_When_No_Types()
	{
		// Arrange
		var mockRepository = new Mock<ISocialMediaTypeRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<SocialMediaTypeEntity>());

		var handler = new GetSocialMediaTypesQueryHandler(mockRepository.Object);
		var query = new GetSocialMediaTypesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result.SocialMediaTypes);
	}

	[Fact]
	public void Constructor_With_Null_Repository_Should_Throw_ArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new GetSocialMediaTypesQueryHandler(null!));
	}
}
