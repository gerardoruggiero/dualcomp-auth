using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.AddressTypes.GetAddressTypes;
using Dualcomp.Auth.Application.AddressTypes.CreateAddressType;
using Dualcomp.Auth.Application.AddressTypes.UpdateAddressType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class AddressTypesControllerTests
{
	[Fact]
	public async Task GetAddressTypes_Should_Return_Ok_With_AddressTypes()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateAddressTypeCommand, CreateAddressTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateAddressTypeCommand, UpdateAddressTypeResult>>();
		
		var expectedResult = new GetAddressTypesResult(new List<AddressTypeItem>
		{
			new AddressTypeItem(Guid.NewGuid().ToString(), "Principal"),
			new AddressTypeItem(Guid.NewGuid().ToString(), "Sucursal"),
			new AddressTypeItem(Guid.NewGuid().ToString(), "Facturación"),
			new AddressTypeItem(Guid.NewGuid().ToString(), "Envío")
		});
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetAddressTypesQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new AddressTypesController(
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
	public async Task GetAddressTypes_Should_Return_BadRequest_On_Exception()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateAddressTypeCommand, CreateAddressTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateAddressTypeCommand, UpdateAddressTypeResult>>();
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetAddressTypesQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new AddressTypesController(
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
		var mockCreateHandler = new Mock<ICommandHandler<CreateAddressTypeCommand, CreateAddressTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateAddressTypeCommand, UpdateAddressTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new AddressTypesController(null!, mockCreateHandler.Object, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_CreateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateAddressTypeCommand, UpdateAddressTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new AddressTypesController(mockQueryHandler.Object, null!, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_UpdateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateAddressTypeCommand, CreateAddressTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new AddressTypesController(mockQueryHandler.Object, mockCreateHandler.Object, null!));
	}
}
