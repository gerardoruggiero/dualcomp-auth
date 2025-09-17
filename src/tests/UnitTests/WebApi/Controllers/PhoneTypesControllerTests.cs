using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes;
using Dualcomp.Auth.Application.PhoneTypes.CreatePhoneType;
using Dualcomp.Auth.Application.PhoneTypes.UpdatePhoneType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class PhoneTypesControllerTests
{
	[Fact]
	public async Task GetPhoneTypes_Should_Return_Ok_With_PhoneTypes()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreatePhoneTypeCommand, CreatePhoneTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdatePhoneTypeCommand, UpdatePhoneTypeResult>>();
		
		var expectedResult = new GetPhoneTypesResult(new List<PhoneTypeItem>
		{
			new PhoneTypeItem(Guid.NewGuid().ToString(), "Principal"),
			new PhoneTypeItem(Guid.NewGuid().ToString(), "Móvil"),
			new PhoneTypeItem(Guid.NewGuid().ToString(), "Fax"),
			new PhoneTypeItem(Guid.NewGuid().ToString(), "WhatsApp")
		});
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetPhoneTypesQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new PhoneTypesController(
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
	public async Task GetPhoneTypes_Should_Return_BadRequest_On_Exception()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreatePhoneTypeCommand, CreatePhoneTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdatePhoneTypeCommand, UpdatePhoneTypeResult>>();
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetPhoneTypesQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new PhoneTypesController(
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
		var mockCreateHandler = new Mock<ICommandHandler<CreatePhoneTypeCommand, CreatePhoneTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdatePhoneTypeCommand, UpdatePhoneTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new PhoneTypesController(null!, mockCreateHandler.Object, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_CreateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdatePhoneTypeCommand, UpdatePhoneTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new PhoneTypesController(mockQueryHandler.Object, null!, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_UpdateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreatePhoneTypeCommand, CreatePhoneTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new PhoneTypesController(mockQueryHandler.Object, mockCreateHandler.Object, null!));
	}
}
