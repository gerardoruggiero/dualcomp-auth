using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.AddressTypes.GetAddressTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class AddressTypesControllerTests
{
	[Fact]
	public async Task GetAddressTypes_Should_Return_Ok_With_AddressTypes()
	{
		// Arrange
		var mockHandler = new Mock<IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult>>();
		var expectedResult = new GetAddressTypesResult(new List<AddressTypeItem>
		{
			new AddressTypeItem("Principal"),
			new AddressTypeItem("Sucursal"),
			new AddressTypeItem("Facturación"),
			new AddressTypeItem("Envío")
		});
		
		mockHandler.Setup(h => h.Handle(It.IsAny<GetAddressTypesQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new AddressTypesController(mockHandler.Object);

		// Act
		var result = await controller.GetAddressTypes(CancellationToken.None);

		// Assert
		Assert.IsType<OkObjectResult>(result);
		var okResult = result as OkObjectResult;
		Assert.Equal(expectedResult, okResult!.Value);
	}

	[Fact]
	public async Task GetAddressTypes_Should_Return_BadRequest_On_Exception()
	{
		// Arrange
		var mockHandler = new Mock<IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult>>();
		mockHandler.Setup(h => h.Handle(It.IsAny<GetAddressTypesQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new AddressTypesController(mockHandler.Object);

		// Act
		var result = await controller.GetAddressTypes(CancellationToken.None);

		// Assert
		Assert.IsType<BadRequestObjectResult>(result);
		var badRequestResult = result as BadRequestObjectResult;
		Assert.NotNull(badRequestResult!.Value);
	}

	[Fact]
	public void Constructor_With_Null_Handler_Should_Throw_ArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new AddressTypesController(null!));
	}
}
