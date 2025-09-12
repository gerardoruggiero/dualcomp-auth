using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class SocialMediaTypesControllerTests
{
	[Fact]
	public async Task GetSocialMediaTypes_Should_Return_Ok_With_SocialMediaTypes()
	{
		// Arrange
		var mockHandler = new Mock<IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult>>();
		var expectedResult = new GetSocialMediaTypesResult(new List<SocialMediaTypeItem>
		{
			new SocialMediaTypeItem(Guid.NewGuid().ToString(), "Facebook"),
			new SocialMediaTypeItem(Guid.NewGuid().ToString(), "Instagram"),
			new SocialMediaTypeItem(Guid.NewGuid().ToString(), "LinkedIn"),
			new SocialMediaTypeItem(Guid.NewGuid().ToString(), "Twitter"),
			new SocialMediaTypeItem(Guid.NewGuid().ToString(), "YouTube")
		});
		
		mockHandler.Setup(h => h.Handle(It.IsAny<GetSocialMediaTypesQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new SocialMediaTypesController(mockHandler.Object);

		// Act
		var result = await controller.GetTypes(CancellationToken.None);

		// Assert
		Assert.IsType<OkObjectResult>(result);
		var okResult = result as OkObjectResult;
		Assert.Equal(expectedResult, okResult!.Value);
	}

	[Fact]
	public async Task GetSocialMediaTypes_Should_Return_BadRequest_On_Exception()
	{
		// Arrange
		var mockHandler = new Mock<IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult>>();
		mockHandler.Setup(h => h.Handle(It.IsAny<GetSocialMediaTypesQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new SocialMediaTypesController(mockHandler.Object);

		// Act
		var result = await controller.GetTypes(CancellationToken.None);

		// Assert
		Assert.IsType<BadRequestObjectResult>(result);
		var badRequestResult = result as BadRequestObjectResult;
		Assert.NotNull(badRequestResult!.Value);
	}

	[Fact]
	public void Constructor_With_Null_Handler_Should_Throw_ArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new SocialMediaTypesController(null!));
	}
}
