using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.Titles.GetTitles;
using Dualcomp.Auth.Application.Titles.CreateTitle;
using Dualcomp.Auth.Application.Titles.UpdateTitle;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class TitlesControllerTests
{
	[Fact]
	public async Task GetTitles_Should_Return_Ok_With_Titles()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetTitlesQuery, GetTitlesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateTitleCommand, CreateTitleResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateTitleCommand, UpdateTitleResult>>();
		
		var expectedResult = new GetTitlesResult(new List<TitleItem>
		{
			new TitleItem(Guid.NewGuid().ToString(), "Ingeniero", "Título de Ingeniero", true),
			new TitleItem(Guid.NewGuid().ToString(), "Doctor", "Título de Doctor", true),
			new TitleItem(Guid.NewGuid().ToString(), "Licenciado", "Título de Licenciado", true),
			new TitleItem(Guid.NewGuid().ToString(), "Técnico", "Título de Técnico", true)
		});
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetTitlesQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new TitlesController(
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
	public async Task GetTitles_Should_Return_BadRequest_On_Exception()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetTitlesQuery, GetTitlesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateTitleCommand, CreateTitleResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateTitleCommand, UpdateTitleResult>>();
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetTitlesQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new TitlesController(
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
		var mockCreateHandler = new Mock<ICommandHandler<CreateTitleCommand, CreateTitleResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateTitleCommand, UpdateTitleResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new TitlesController(null!, mockCreateHandler.Object, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_CreateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetTitlesQuery, GetTitlesResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateTitleCommand, UpdateTitleResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new TitlesController(mockQueryHandler.Object, null!, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_UpdateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetTitlesQuery, GetTitlesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateTitleCommand, CreateTitleResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new TitlesController(mockQueryHandler.Object, mockCreateHandler.Object, null!));
	}
}

