using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes;
using Dualcomp.Auth.Application.SocialMediaTypes.CreateSocialMediaType;
using Dualcomp.Auth.Application.SocialMediaTypes.UpdateSocialMediaType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class SocialMediaTypesControllerTests
{
	[Fact]
	public async Task GetSocialMediaTypes_Should_Return_Ok_With_SocialMediaTypes()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateSocialMediaTypeCommand, CreateSocialMediaTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateSocialMediaTypeCommand, UpdateSocialMediaTypeResult>>();
		
		var expectedResult = new GetSocialMediaTypesResult(new List<SocialMediaTypeItem>
		{
			new SocialMediaTypeItem(Guid.NewGuid().ToString(), "Facebook", "Página de Facebook", true),
			new SocialMediaTypeItem(Guid.NewGuid().ToString(), "Instagram", "Perfil de Instagram", true),
			new SocialMediaTypeItem(Guid.NewGuid().ToString(), "LinkedIn", "Página de LinkedIn", true),
			new SocialMediaTypeItem(Guid.NewGuid().ToString(), "Twitter", "Perfil de Twitter", true),
			new SocialMediaTypeItem(Guid.NewGuid().ToString(), "YouTube", "Canal de YouTube", true)
		});
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetSocialMediaTypesQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new SocialMediaTypesController(
			mockQueryHandler.Object, 
			mockCreateHandler.Object, 
			mockUpdateHandler.Object);

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
		var mockQueryHandler = new Mock<IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateSocialMediaTypeCommand, CreateSocialMediaTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateSocialMediaTypeCommand, UpdateSocialMediaTypeResult>>();
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetSocialMediaTypesQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new SocialMediaTypesController(
			mockQueryHandler.Object, 
			mockCreateHandler.Object, 
			mockUpdateHandler.Object);

		// Act
		var result = await controller.GetTypes(CancellationToken.None);

		// Assert
		Assert.IsType<BadRequestObjectResult>(result);
		var badRequestResult = result as BadRequestObjectResult;
		Assert.NotNull(badRequestResult!.Value);
	}

	[Fact]
	public void Constructor_With_Null_QueryHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockCreateHandler = new Mock<ICommandHandler<CreateSocialMediaTypeCommand, CreateSocialMediaTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateSocialMediaTypeCommand, UpdateSocialMediaTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new SocialMediaTypesController(null!, mockCreateHandler.Object, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_CreateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateSocialMediaTypeCommand, UpdateSocialMediaTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new SocialMediaTypesController(mockQueryHandler.Object, null!, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_UpdateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateSocialMediaTypeCommand, CreateSocialMediaTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new SocialMediaTypesController(mockQueryHandler.Object, mockCreateHandler.Object, null!));
	}
}
