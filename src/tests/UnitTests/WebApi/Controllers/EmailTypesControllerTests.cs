using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.EmailTypes.GetEmailTypes;
using Dualcomp.Auth.Application.EmailTypes.CreateEmailType;
using Dualcomp.Auth.Application.EmailTypes.UpdateEmailType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class EmailTypesControllerTests
{
	[Fact]
	public async Task GetEmailTypes_Should_Return_Ok_With_EmailTypes()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetEmailTypesQuery, GetEmailTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateEmailTypeCommand, CreateEmailTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateEmailTypeCommand, UpdateEmailTypeResult>>();
		
		var expectedResult = new GetEmailTypesResult(new List<EmailTypeItem>
		{
			new EmailTypeItem(Guid.NewGuid().ToString(), "Principal"),
			new EmailTypeItem(Guid.NewGuid().ToString(), "Facturación"),
			new EmailTypeItem(Guid.NewGuid().ToString(), "Soporte"),
			new EmailTypeItem(Guid.NewGuid().ToString(), "Comercial")
		});
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetEmailTypesQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new EmailTypesController(
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
	public async Task GetEmailTypes_Should_Return_BadRequest_On_Exception()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetEmailTypesQuery, GetEmailTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateEmailTypeCommand, CreateEmailTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateEmailTypeCommand, UpdateEmailTypeResult>>();
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetEmailTypesQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new EmailTypesController(
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
		var mockCreateHandler = new Mock<ICommandHandler<CreateEmailTypeCommand, CreateEmailTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateEmailTypeCommand, UpdateEmailTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new EmailTypesController(null!, mockCreateHandler.Object, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_CreateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetEmailTypesQuery, GetEmailTypesResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateEmailTypeCommand, UpdateEmailTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new EmailTypesController(mockQueryHandler.Object, null!, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_UpdateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetEmailTypesQuery, GetEmailTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateEmailTypeCommand, CreateEmailTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new EmailTypesController(mockQueryHandler.Object, mockCreateHandler.Object, null!));
	}
}
